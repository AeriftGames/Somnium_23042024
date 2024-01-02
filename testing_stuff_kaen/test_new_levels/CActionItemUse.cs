using Godot;
using System;

public partial class CActionItemUse : Node3D
{
	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
	}

	public virtual void UseAction()
	{
	}

	public virtual void HoverAction(bool newHover) 
	{
		GD.Print("Hover: " + newHover);
	}
}
