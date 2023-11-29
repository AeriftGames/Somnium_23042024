using Godot;
using System;

public partial class CLandPlayerState : CState
{

    public override void Enter()
    {
        // await finish land anim and  go to idle
        CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterLandComponent().DoLandEffect();

        WaitForLandComplete();
    }

    public async void WaitForLandComplete()
    {
        await ToSignal(CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterLandComponent(),
            nameof(CCharacterLandComponent.LandComplete));

        EmitSignal(SignalName.Transition, "IdlePlayerState");
    }

    public override void Update(float delta)
    {

    }
}
