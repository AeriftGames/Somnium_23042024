using Godot;
using System;

public partial class CJumpPlayerState : CState
{
    [Export] AnimationPlayer ANIMATION;
    public override void Enter()
    {
        ANIMATION.Pause();
    }

    public override void Update(float delta)
    {
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterMovementComponent().GetIsOnFloor())
            EmitSignal(SignalName.Transition, "IdlePlayerState");
    }
}
