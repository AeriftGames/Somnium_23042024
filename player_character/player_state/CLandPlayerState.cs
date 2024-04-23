using Godot;
using System;

public partial class CLandPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

        FPSCharacterAction FPSMoveAction = ourCharacterBase as FPSCharacterAction;
        if (FPSMoveAction != null)
        {
            if (FPSMoveAction.GetJumpLandEffectComponent() != null)
            { FPSMoveAction.GetJumpLandEffectComponent().ApplyEffectLand(); }

            // Apply Land Fov effect now
            if (FPSMoveAction.GetCharacterFovComponent() != null)
            { FPSMoveAction.GetCharacterFovComponent().SetLandFovNow(); }

            // Remove Stamina
            if (FPSMoveAction.GetStaminaComponent() != null)
            { FPSMoveAction.GetStaminaComponent().RemoveStaminaLand(); }
        }
    }

    public override void Update(float delta)
    {
        if (ourCharacterBase.Velocity.Y == 0.0f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
        { EmitSignal(nameof(Transition), "IdlePlayerState"); }

        else if (ourCharacterBase.Velocity.Y == 0.0f &&
            ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == true)
        { EmitSignal(nameof(Transition), "IdleCrouchPlayerState"); }
    }
}
