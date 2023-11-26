using Godot;
using System;

public partial class CIdlePlayerState : CState
{
    public override void Update(float delta)
    {
        if(CGameMaster.GM.GetGame().GetFPSCharacterBase().Velocity.Length() >= 0.02f)
        {
            EmitSignal(SignalName.Transition, "WalkingPlayerState");
        }
    }
}
