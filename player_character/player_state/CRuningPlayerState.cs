using Godot;
using System;

public partial class CRuningPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

    }

    public override void Update(float delta)
    {
        if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "CrouchMovePlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() <= 3.19f)
        { EmitSignal(nameof(Transition), "WalkingPlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() < 0.01f)
        { EmitSignal(nameof(Transition), "IdlePlayerState"); }

        else if (ourCharacterBase.Velocity.Y > 0.0f)
        { EmitSignal(nameof(Transition), "JumpPlayerState"); }

        else if (ourCharacterBase.Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }
    }
}
