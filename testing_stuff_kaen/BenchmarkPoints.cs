using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BenchmarkPoints : Node3D
{
	PathFollow3D pathFollowPos = null;
	PathFollow3D pathFollowLook = null;
	Camera3D cam = null;
	Vector3 interpPos;
	Vector3 interpLook;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFollowPos = GetNode<PathFollow3D>("Path3D/PathFollow3D_pos_point");
		pathFollowLook = GetNode<PathFollow3D>("Path3D/PathFollow3D_look_point");
		cam = GetNode<Camera3D>("Camera3D");

		interpPos = pathFollowPos.GlobalPosition;
		interpLook = pathFollowLook.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		pathFollowPos.Progress += (float)delta;
		pathFollowLook.Progress += (float)delta;

		interpPos = interpPos.Lerp(pathFollowPos.GlobalPosition, 0.5f * (float)delta);
        interpLook = interpLook.Lerp(pathFollowLook.GlobalPosition, 0.5f * (float)delta);

        cam.GlobalPosition = interpPos;
		cam.LookAtFromPosition(cam.GlobalPosition,interpLook);
	}
}
