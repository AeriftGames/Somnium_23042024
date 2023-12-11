using Godot;
using System;

public partial class CLandPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

        FPSCharacterMoveAnim FPSMoveAnim = ourCharacterBase as FPSCharacterMoveAnim;
        if (FPSMoveAnim != null)
        {
            if (FPSMoveAnim.GetJumpLandEffectComponent() != null)
            { FPSMoveAnim.GetJumpLandEffectComponent().ApplyEffectLand(); }

            // Apply Land Fov effect now
            if (FPSMoveAnim.GetCharacterFovComponent != null)
            { ourCharacterBase.GetCharacterFovComponent().SetLandFovNow(); }
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
