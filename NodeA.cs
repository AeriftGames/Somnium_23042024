using Godot;
using System;

public partial class NodeA : Node3D
{
	public string text_test = "blablabla";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public string GetTestText()
	{
		return text_test;
	}
}
