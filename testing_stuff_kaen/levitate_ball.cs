using Godot;
using System;

public partial class levitate_ball : MeshInstance3D
{
	AnimationPlayer player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.Play("levitate");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
