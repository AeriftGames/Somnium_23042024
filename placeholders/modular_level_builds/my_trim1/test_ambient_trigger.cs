using Godot;
using System;

public partial class test_ambient_trigger : Area3D
{
	AudioStreamPlayer3D test;
	[Export] public bool doOnce = true;
	bool isPlayed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		test = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_body_entered(Node3D body)
	{
		if(body.IsClass("CharacterBody3D"))
		{
			GD.Print("character entered to area");

			if(doOnce && !isPlayed)
                test.Play();
			else if(!doOnce)
				test.Play();

			isPlayed = true;
		}

	}
}
