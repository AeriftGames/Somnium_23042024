using Godot;
using System;

public partial class CState : Node
{
    [Signal] public delegate void TransitionEventHandler(StringName newStateName);

    public virtual void Enter() { }
	public virtual void Exit() { }
	public virtual void Update(float delta) { }
	public virtual void PhysicsUpdate(float delta) { }
}
