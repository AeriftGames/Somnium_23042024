using Godot;
using System;

public partial class CCharacterMovementComponent : CBaseComponent
{
    public enum ESpeedMoveType { SPEED_WALK,SPEED_SPRINT,SPEED_CROUCH,SPEED_CROUCH_DYNAMIC }

    [Export] public float SPEED_WALK = 2.0f;
    [Export] public float SPEED_SPRINT = 3.0f;
    [Export] public float SPEED_CROUCH = 1.3f;
    [Export] public float SPEED_CROUCH_DYNAMIC = 0.7f;

    [Export] public float ACCELERATION = 4f;
    [Export] public float DECCLERATION = 8f;

    [Export] public float JUMP_VELOCITY = 4.5f;

    [Export] public bool CAN_MOVEINFALL = true;
    [Export] public float MOVESPEED_INFALL = 1.4f;
    [Export] public float DECCELERATE_INFALL = 1.0f;
    [Export] public bool ENABLE_LIMITVELOCITY_AFTERLANDED = true;
    [Export] public float LANDING_LIMIT_MOVEVELOCITY = 1.5f;

    private float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector3 WorkVelocity;
    private float Speed = 0.0f;
    private ESpeedMoveType ActualSpeedMoveType;

    private Vector2 InputDir = Vector2.Zero;
    private Vector3 Direction = Vector3.Zero;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        //SetMoveSpeed("WALK");
        SetMoveSpeed(ESpeedMoveType.SPEED_WALK);
    }

    public void UpdateMove(double delta)
    {
        // get input actions for input dir and calculate direction
        InputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        Direction = Direction.Lerp(ourCharacterBase.Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y).Normalized(), (float)delta * 60.0f);

        // get actual character velocity
        WorkVelocity = ourCharacterBase.Velocity;
        WorkVelocity.Y = 0f;    // disable gravity at this moment

        if (Direction != Vector3.Zero)
        {
            // for move on ground - with input
            if (GetIsOnFloor())
            {
                // new move fix pro vyreseni bugu se zdi
                if(ourCharacterBase.IsOnWall() && InputDir != Vector2.Zero)
                {
                    var newDir = ourCharacterBase.GetWallNormal().Slide(Direction).Normalized();
                    newDir.Y = 0.0f;
                    WorkVelocity = WorkVelocity.Lerp(newDir * Speed, ACCELERATION * (float)delta);
                }
                else
                {
                    WorkVelocity = WorkVelocity.Lerp(Direction * Speed, ACCELERATION * (float)delta);
                }
            }
            // for fall - with input
            else if (CAN_MOVEINFALL)
            {
                // add additional move velocity and set limit length for final velocity
                WorkVelocity += (Direction * MOVESPEED_INFALL) * (float)delta;
                WorkVelocity = WorkVelocity.LimitLength(Speed);
            }
        }
        else
        {
            // for move on ground - no input
            if (GetIsOnFloor())
            {
                WorkVelocity.X = Mathf.MoveToward(ourCharacterBase.Velocity.X, 0, DECCLERATION * (float)delta);
                WorkVelocity.Z = Mathf.MoveToward(ourCharacterBase.Velocity.Z, 0, DECCLERATION * (float)delta);
            }
            // for fall - no input
            else if (CAN_MOVEINFALL)
            {
                WorkVelocity.X = Mathf.MoveToward(ourCharacterBase.Velocity.X, 0, DECCELERATE_INFALL * (float)delta);
                WorkVelocity.Z = Mathf.MoveToward(ourCharacterBase.Velocity.Z, 0, DECCELERATE_INFALL * (float)delta);
            }
        }

        // limit velocity after landing
        if (ENABLE_LIMITVELOCITY_AFTERLANDED && GetIsOnFloor() &&
            ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "LandPlayerState")
        { WorkVelocity = WorkVelocity.LimitLength(LANDING_LIMIT_MOVEVELOCITY); }  // zmensi aktualni velocity

        // add get back original velocity y
        WorkVelocity.Y = ourCharacterBase.Velocity.Y;

        // add the gravity.
        if (!ourCharacterBase.IsOnFloor())
            WorkVelocity.Y -= Gravity * (float)delta;
    }

    public bool GetIsOnFloor() { return ourCharacterBase.IsOnFloor(); }

    public void ApplyWorkVelocity()
    {
        ourCharacterBase.Velocity = WorkVelocity;
        ourCharacterBase.MoveAndSlide();
    }

    public bool CheckAndApplyJump(StringName newInput)
    {
        if (ourCharacterBase.GetCharacterMovementComponent() == null) return false;
        bool isOnFloor = ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor();

        if (ourCharacterBase.GetCharacterCrouchComponent() == null) return false;
        bool isCrouch = ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched();

        if (Input.IsActionJustPressed(newInput) && isOnFloor && !isCrouch)
        {
            WorkVelocity.Y = JUMP_VELOCITY;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMoveSpeed(ESpeedMoveType newSpeedMoveType)
    {
        ActualSpeedMoveType = newSpeedMoveType;

        switch (newSpeedMoveType)
        {
            case ESpeedMoveType.SPEED_WALK:
                { Speed = SPEED_WALK; break; }
            case ESpeedMoveType.SPEED_SPRINT:
                { Speed = SPEED_SPRINT; break; }
            case ESpeedMoveType.SPEED_CROUCH:
                { Speed = SPEED_CROUCH; break; }
            case ESpeedMoveType.SPEED_CROUCH_DYNAMIC:
                { Speed = SPEED_CROUCH_DYNAMIC; break; }
            default:
                break;
        }
    }

    public ESpeedMoveType GetActualSpeedMoveType() { return ActualSpeedMoveType; }

    public float GetWantSpeed() { return Speed; }
    public float GetRealSpeed()
    {
        Vector3 moveVelocity = new Vector3(ourCharacterBase.GetRealVelocity().X, 0, ourCharacterBase.GetRealVelocity().Z);
        return float.Round(MathF.Abs(moveVelocity.Length()), 1);
    }

    public float GetRealUnroundedSpeed()
    {
        Vector3 moveVelocity = new Vector3(ourCharacterBase.GetRealVelocity().X, 0, ourCharacterBase.GetRealVelocity().Z);
        return moveVelocity.Length();
    }


    public Vector2 GetInputDir() { return InputDir; }
    public Vector3 GetDirection() {  return Direction; }

    public bool GetIsWantSprint() { 
        if (Speed == SPEED_SPRINT) return true;
        else return false;
    }
}
