using Godot;
using System;

public partial class CJumpPlayerState : CState
{
    [Export] AnimationPlayer ANIMATION;
    public override void Enter()
    {
        if (ANIMATION.CurrentAnimation == "Running" || ANIMATION.CurrentAnimation == "Walking")
            CGameMaster.GM.GetGame().GetFPSCharacterBase().movementAnimationLastTime = ANIMATION.CurrentAnimationPosition;

        ANIMATION.Pause();
    }

    public override void Update(float delta)
    {
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterMovementComponent().GetIsOnFloor())
            EmitSignal(SignalName.Transition, "LandPlayerState");
        else if (CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Y < 0.0f)
            EmitSignal(SignalName.Transition, "FallPlayerState");
    }
}
