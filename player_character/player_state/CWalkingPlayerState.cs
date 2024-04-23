using Godot;
using System;

public partial class CWalkingPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update(float delta)
    {
        if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "CrouchMovePlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 3.2f)
        { EmitSignal(nameof(Transition), "RunningPlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() < 0.01f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
        { EmitSignal(nameof(Transition), "IdlePlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() < 0.01f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "IdleCrouchPlayerState"); }

        else if (ourCharacterBase.Velocity.Y > 0.0f)
        { EmitSignal(nameof(Transition), "JumpPlayerState"); }

        else if (ourCharacterBase.Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }
    }
}
