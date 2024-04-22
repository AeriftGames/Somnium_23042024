using Godot;
using System;

public partial class CJumpPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

        FPSCharacterAction FPSAction = ourCharacterBase as FPSCharacterAction;
        if (FPSAction != null)
        {
            if (FPSAction.GetJumpLandEffectComponent() != null)
            { FPSAction.GetJumpLandEffectComponent().ApplyEffectJump(); }

            if (FPSAction.GetStaminaComponent() != null)
            { FPSAction.GetStaminaComponent().RemoveStaminaJump(); }
        }
    }

    public override void Update(float delta)
    {
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }
    }
}
