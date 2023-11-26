using Godot;
using System;

public partial class CCharacterMovementComponent : Node
{
    [Export] float SPEED_WALK = 3.0f;
    [Export] float SPEED_SPRINT = 5.0f;
    [Export] float SPEED_CROUCH = 1.5f;

    [Export] float JUMP_VELOCITY = 4.5f;
    [Export] float LERP_SPEED = 10.0f;

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private FpsCharacterBase ourCharacterBase = null;
    private Vector3 workVelocity;
    private float Speed = 0.0f;

    private Vector3 direction = Vector3.Zero;

    public void PostInit(FpsCharacterBase newCharacterBase)
    { 
        ourCharacterBase = newCharacterBase;
        SetMoveSpeed("WALK");
    }

    public void UpdateMove(double delta)
    {
        workVelocity = ourCharacterBase.Velocity;

        // Add the gravity.
        if (!ourCharacterBase.IsOnFloor())
            workVelocity.Y -= gravity * (float)delta;

        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        direction = direction.Lerp(ourCharacterBase.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized(),(float)delta*LERP_SPEED);
        if (direction != Vector3.Zero)
        {
            workVelocity.X = direction.X * Speed;
            workVelocity.Z = direction.Z * Speed;
        }
        else
        {
            workVelocity.X = Mathf.MoveToward(ourCharacterBase.Velocity.X, 0, Speed);
            workVelocity.Z = Mathf.MoveToward(ourCharacterBase.Velocity.Z, 0, Speed);
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

    public float GetSpeed() { return Speed; }
    public float GetRealSpeed()
    {
        Vector3 moveVelocity = new Vector3(ourCharacterBase.GetRealVelocity().X, 0, ourCharacterBase.GetRealVelocity().Z);
        return float.Round(MathF.Abs(moveVelocity.Length()), 1);
    }
}
