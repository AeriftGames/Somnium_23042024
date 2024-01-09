using Godot;
using System;

public partial class CIdleCrouchPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();
        /*
        ourCharacterBase.GetCharacterMovementComponent().SetMoveSpeed(
            CCharacterMovementComponent.ESpeedMoveType.SPEED_CROUCH);*/
    }

    public override void Update(float delta)
    {
        if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() >= 0.01f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "CrouchMovePlayerState"); }

        else if (ourCharacterBase.Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }

        else if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
        { EmitSignal(nameof(Transition), "IdlePlayerState"); }
    }
}
