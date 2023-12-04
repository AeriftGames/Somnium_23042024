using Godot;
using System;

public partial class CIdlePlayerState : CState
{

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update(float delta)
    {
        if(ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() >= 0.01f && 
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
        { EmitSignal(nameof(Transition),"WalkingPlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() >= 0.01f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "CrouchMovePlayerState"); }

        else if (ourCharacterBase.Velocity.Y > 0.0f)
        { EmitSignal(nameof(Transition), "JumpPlayerState"); }

        else if (ourCharacterBase.Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }

        else if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() < 0.01f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "IdleCrouchPlayerState"); }
    }
}
