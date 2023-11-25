using Godot;
using System;

public partial class CCharacterMovementComponent : Node
{
    [Export] float Speed = 5.0f;
    [Export] float JumpVelocity = 4.5f;

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private FpsCharacterBase ourCharacterBase = null;

    public void PostInit(FpsCharacterBase newCharacterBase) { ourCharacterBase = newCharacterBase; }

    public  void UpdateMove(double delta)
    {
        Vector3 velocity = ourCharacterBase.Velocity;

        // Add the gravity.
        if (!ourCharacterBase.IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && ourCharacterBase.IsOnFloor())
            velocity.Y = JumpVelocity;

        Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
        Vector3 direction = (ourCharacterBase.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(ourCharacterBase.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(ourCharacterBase.Velocity.Z, 0, Speed);
        }

        ourCharacterBase.Velocity = velocity;
        ourCharacterBase.MoveAndSlide();
    }
}
