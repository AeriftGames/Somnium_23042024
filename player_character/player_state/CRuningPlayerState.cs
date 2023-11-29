using Godot;
using System;

public partial class CRuningPlayerState : CState
{
    [Export] public AnimationPlayer ANIMATION;
    [Export] public float TOP_ANIM_SPEED = 2.2f;

    public override void Enter()
    {

        double oldTime = oldTime = ANIMATION.CurrentAnimationPosition;

        ANIMATION.Play("Running", -1.0f, 1.0f);
        ANIMATION.Seek(oldTime);
    }

    public override void Update(float delta)
    {
        SetAnimationSpeed(CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Length());

        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Length() < 2.2f)
            EmitSignal(SignalName.Transition, "WalkingPlayerState");

        else if (CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterMovementComponent().GetIsOnFloor() == false)
            EmitSignal(SignalName.Transition, "JumpPlayerState");
    }

    public void SetAnimationSpeed(float newSpeed)
    {
        float alpha = Mathf.Remap(newSpeed, 0.0f, CGameMaster.GM.GetGame().GetFPSCharacterBase()
            .GetCharacterMovementComponent().SPEED_WALK, 0.0f, 1.0f);

        ANIMATION.SpeedScale = Mathf.Lerp(0.0f, TOP_ANIM_SPEED, alpha);
    }
}
