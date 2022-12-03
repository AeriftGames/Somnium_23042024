using Godot;
using System;

public partial class UseFunc : Node3D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void use()
	{
		GD.Print("UseFunc.use!");
	}
}
