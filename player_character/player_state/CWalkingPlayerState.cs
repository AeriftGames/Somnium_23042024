using Godot;
using System;

public partial class CWalkingPlayerState : CState
{
    [Export] public AnimationPlayer ANIMATION;
    [Export] public float TOP_ANIM_SPEED = 2.2f;

    public override void Enter()
    {
        ANIMATION.Play("Walking",-1.0f,1.0f);
    }

    public override void Update(float delta)
    {
        SetAnimationSpeed(CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Length());
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Length() < 0.1f)
            EmitSignal(SignalName.Transition, "IdlePlayerState");
    }

    public void SetAnimationSpeed(float newSpeed)
    {
        float alpha = Mathf.Remap(newSpeed, 0.0f, CGameMaster.GM.GetGame().GetFPSCharacterBase()
            .GetCharacterMovementComponent().SPEED_WALK, 0.0f, 1.0f);

        ANIMATION.SpeedScale = Mathf.Lerp(0.0f,TOP_ANIM_SPEED, alpha);
    }
}
