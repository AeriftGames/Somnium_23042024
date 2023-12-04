using Godot;
using System;

public partial class CFallPlayerState : CState
{
    public override void Enter()
    {
        base.Enter();

    }

    public override void Update(float delta)
    {
        if (ourCharacterBase.Velocity.Y == 0.0f && 
            ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor())
        { EmitSignal(nameof(Transition), "LandPlayerState"); }
    }
}
