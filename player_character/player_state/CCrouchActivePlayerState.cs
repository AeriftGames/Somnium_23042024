using Godot;
using System;

public partial class CCrouchActivePlayerState : CState
{
    public override void Enter()
    {
        base.Enter();
/*
        ourCharacterBase.GetCharacterMovementComponent().SetMoveSpeed(
            CCharacterMovementComponent.ESpeedMoveType.SPEED_CROUCH_DYNAMIC);*/
    }

    public override void Update(float delta)
    {
        if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouchExtra() == false)
        { EmitSignal(nameof(Transition), "IdleCrouchPlayerState"); }

        else if (ourCharacterBase.Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }
    }
}
