using Godot;
using System;

public partial class CFallPlayerState : CState
{
    public override void Enter()
    {

    }

    public override void Update(float delta)
    {
        if (CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterMovementComponent().GetIsOnFloor())
            EmitSignal(SignalName.Transition, "LandPlayerState");
    }
}
