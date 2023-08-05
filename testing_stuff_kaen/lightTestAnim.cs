using Godot;
using System;

public partial class lightTestAnim : OmniLight3D
{
	[Export] public string animToPlay = "";
	AnimationPlayer player;

	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.Play(animToPlay);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
