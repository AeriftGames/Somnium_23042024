using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BenchmarkPoints : Node3D
{
	PathFollow3D pathFollowPos = null;
	PathFollow3D pathFollowLook = null;
	BenchmarkCamera cam = null;
	Vector3 interpPos;
	Vector3 interpLook;

	[Export] public float speed = 3.0f;
	[Export] public float speed_interp = 0.8f;

	private bool updateMove = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFollowPos = GetNode<PathFollow3D>("Path3D/PathFollow3D_pos_point");
		pathFollowLook = GetNode<PathFollow3D>("Path3D/PathFollow3D_look_point");
		cam = GetNode<BenchmarkCamera>("BenchmarkCamera");

		interpPos = pathFollowPos.GlobalPosition;
		interpLook = pathFollowLook.GlobalPosition;

        //GameMaster.GM.GetBenchmarkSystem().StartBenchmarkLevel("res://levels/worldlevel_demo_extend_benchmark.tscn", "benchmark_level_1");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (updateMove == false) return;

		pathFollowPos.Progress += speed*(float)delta;
		pathFollowLook.Progress += speed*(float)delta;

		interpPos = interpPos.Lerp(pathFollowPos.GlobalPosition, speed_interp * (float)delta);
        interpLook = interpLook.Lerp(pathFollowLook.GlobalPosition, speed_interp * (float)delta);

        cam.GlobalPosition = interpPos;
		cam.LookAtFromPosition(cam.GlobalPosition,interpLook);

		// end
		if(pathFollowPos.ProgressRatio > 0.98f)
		{
			updateMove = false;

			// pokud nejsme na konci benchmarku (mysleno neprobehly vsechny kvality)
			if (!GameMaster.GM.GetBenchmarkSystem().BenchmarkEnd)
				GameMaster.GM.GetBenchmarkSystem().NextBenchmarkQuality();

        }
	}
}
