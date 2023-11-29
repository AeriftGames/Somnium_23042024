using Godot;
using System;

public partial class CIdlePlayerState : CState
{
    [Export] AnimationPlayer ANIMATION;
    [Export] public float TOP_ANIM_SPEED = 2.2f;

    public override void Enter()
    {
        if (ANIMATION.CurrentAnimation == "Running" || ANIMATION.CurrentAnimation == "Walking")
            CGameMaster.GM.GetGame().GetFPSCharacterBase().movementAnimationLastTime = ANIMATION.CurrentAnimationPosition;

        ANIMATION.Pause();
    }

    public override void Update(float delta)
    {
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Length() >= 0.1f &&
            CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterMovementComponent().GetIsOnFloor())
            EmitSignal(SignalName.Transition, "WalkingPlayerState");

        else if (CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterMovementComponent().GetIsOnFloor() == false)
            EmitSignal(SignalName.Transition, "JumpPlayerState");
    }
}
