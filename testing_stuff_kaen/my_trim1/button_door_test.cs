using Godot;
using Godot.Collections;
using System;
using System.Security.AccessControl;

public partial class button_door_test : BasicButtonObject
{
    MeshInstance3D meshButton;
    AnimationPlayer animPlayer;
    public override void _Ready()
	{
        base._Ready();

        meshButton = GetNode<MeshInstance3D>("button");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void UseButtonEffect()
    {
        base.UseButtonEffect();

        animPlayer.Play("buttonPressed");
    }
}
