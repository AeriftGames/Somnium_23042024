using Godot;
using System;

public partial class CJumpPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

        FPSCharacterMoveAnim FPSMoveAnim = ourCharacterBase as FPSCharacterMoveAnim;
        if(FPSMoveAnim != null)
        {
            if(FPSMoveAnim.GetJumpLandEffectComponent() != null)
            { FPSMoveAnim.GetJumpLandEffectComponent().ApplyEffectJump(); }
        }
    }

    public override void Update(float delta)
    {
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Y < 0.0f)
        { EmitSignal(nameof(Transition), "FallPlayerState"); }
    }
}
