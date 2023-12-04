using Godot;
using System;

public partial class CState : Node
{
	protected FpsCharacterBase ourCharacterBase = null;

    [Signal] public delegate void TransitionEventHandler(StringName newStateName);

    public virtual void Enter() { ourCharacterBase = CGameMaster.GM.GetGame().GetFPSCharacterBase(); }
	public virtual void Exit() { }
	public virtual void Update(float delta) { }
	public virtual void PhysicsUpdate(float delta) { }
}
