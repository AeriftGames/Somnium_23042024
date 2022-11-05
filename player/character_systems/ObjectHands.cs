using Godot;
using System;

public partial class ObjectHands : Node3D
{
	public Node3D objectFlashlight = null;

	public override void _Ready()
	{
		objectFlashlight = GetNode<Node3D>("ObjectFlashlight");
	}

	public override void _Process(double delta)
	{

	}
}
