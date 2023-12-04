using Godot;
using System;

public partial class CCharacterMovementComponent : CBaseComponent
{
    [Export] public float SPEED_WALK = 2.0f;
    [Export] public float SPEED_SPRINT = 3.0f;
    [Export] public float SPEED_CROUCH = 1.3f;

    [Export] public float ACCELERATION = 4f;
    [Export] public float DECCLERATION = 8f;

    [Export] public float JUMP_VELOCITY = 4.5f;

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector3 workVelocity;
    private float Speed = 0.0f;

    private Vector2 inputDir = Vector2.Zero;
    private Vector3 direction = Vector3.Zero;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        SetMoveSpeed("WALK");
    }

    public void UpdateMove(double delta)
    {
        workVelocity = ourCharacterBase.Velocity;

        // Add the gravity.
        if (!ourCharacterBase.IsOnFloor())
            workVelocity.Y -= gravity * (float)delta;

        if (Input.IsActionPressed("Sprint") && GetIsOnFloor() && ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
            Speed = SPEED_SPRINT;
        else if (GetIsOnFloor() && ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
            Speed = SPEED_CROUCH;
        else if (GetIsOnFloor() && ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
            Speed = SPEED_WALK;

        inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        direction = direction.Lerp(ourCharacterBase.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized(),(float)delta*60.0f);
        if (direction != Vector3.Zero)
        {
            if(GetIsOnFloor())
            {
                workVelocity.X = float.Lerp(workVelocity.X, direction.X * Speed, ACCELERATION * (float)delta);
                workVelocity.Z = float.Lerp(workVelocity.Z, direction.Z * Speed, ACCELERATION * (float)delta);
            }
        }
        else
        {
            if (GetIsOnFloor())
            {
                workVelocity.X = Mathf.MoveToward(ourCharacterBase.Velocity.X, 0, DECCLERATION * (float)delta);
                workVelocity.Z = Mathf.MoveToward(ourCharacterBase.Velocity.Z, 0, DECCLERATION * (float)delta);
            }
        }
    }

    public bool GetIsOnFloor() { return ourCharacterBase.IsOnFloor(); }

    public void ApplyWorkVelocity()
    {
        ourCharacterBase.Velocity = workVelocity;
        ourCharacterBase.MoveAndSlide();
    }

    public void CheckAndApplyJump(StringName newInput)
    {
        if (ourCharacterBase.GetCharacterMovementComponent() == null) return;
        bool isOnFloor = ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor();

        if (ourCharacterBase.GetCharacterCrouchComponent() == null) return;
        bool isCrouch = ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched();

        if (Input.IsActionJustPressed(newInput) && isOnFloor && !isCrouch)
            workVelocity.Y = JUMP_VELOCITY;
    }

    public void SetMoveSpeed(string newSpeedName)
    {
        switch (newSpeedName)
        {
            case "WALK": Speed = SPEED_WALK; break;
            case "SPRINT": Speed = SPEED_SPRINT; break;
            case "CROUCH": Speed = SPEED_CROUCH; break;
            default: break;
        }
    }

    public float GetWantSpeed() { return Speed; }
    public float GetRealSpeed()
    {
        Vector3 moveVelocity = new Vector3(ourCharacterBase.GetRealVelocity().X, 0, ourCharacterBase.GetRealVelocity().Z);
        return float.Round(MathF.Abs(moveVelocity.Length()), 1);
    }

    public Vector2 GetInputDir() { return inputDir; }
    public Vector3 GetDirection() {  return direction; }
}
