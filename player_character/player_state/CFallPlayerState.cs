using Godot;
using System;

public partial class CFallPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

        FPSCharacterMoveAnim FPSMoveAnim = ourCharacterBase as FPSCharacterMoveAnim;
        if (FPSMoveAnim != null)
        {
            if (FPSMoveAnim.GetJumpLandEffectComponent() != null)
            { FPSMoveAnim.GetJumpLandEffectComponent().SetStartFallingNow(); }
        }
    }

    public override void Update(float delta)
    {
        if (/*ourCharacterBase.Velocity.Y == 0.0f && */
            ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor())
        { EmitSignal(nameof(Transition), "LandPlayerState"); }
    }
}
