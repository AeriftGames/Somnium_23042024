using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class BenchmarkPoints : Node3D
{
	PathFollow3D pathFollowPos = null;
	PathFollow3D pathFollowLook = null;
	BenchmarkCameraBody benchmarkBody = null;
	Vector3 interpPos;
	Vector3 interpLook;

	[Export] public float speed = 3.0f;
	[Export] public float speed_interp = 0.8f;

	private bool updateMove = false;

	Godot.Timer FpsTimer = new Timer();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFollowPos = GetNode<PathFollow3D>("Path3D/PathFollow3D_pos_point");
		pathFollowLook = GetNode<PathFollow3D>("Path3D/PathFollow3D_look_point");
		benchmarkBody = GetNode<BenchmarkCameraBody>("BenchmarkCameraBody");

		interpPos = pathFollowPos.GlobalPosition;
		interpLook = pathFollowLook.GlobalPosition;

		// FPS
		FpsTimer.WaitTime = CGameMaster.GM.GetBenchmark().AddFpsDataTimer;
        FpsTimer.OneShot = false;
		FpsTimer.Connect("timeout", new Callable(this, "AddFpsNow"));
		AddChild(FpsTimer);
		FpsTimer.Start();

		PostInit();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (updateMove == false) return;

		pathFollowPos.Progress += speed*(float)delta;
		pathFollowLook.Progress += speed*(float)delta;

		interpPos = interpPos.Lerp(pathFollowPos.GlobalPosition, speed_interp * (float)delta);
        interpLook = interpLook.Lerp(pathFollowLook.GlobalPosition, speed_interp * (float)delta);

        benchmarkBody.GlobalPosition = interpPos;
		benchmarkBody.LookAtFromPosition(benchmarkBody.GlobalPosition,interpLook);

		// Benchmark update state
		if (pathFollowPos.ProgressRatio > 0.99f)
		{
			updateMove = false;
			FpsTimer.Stop();

			// Posleme globalni signal ze benchmark presset se provedl do konce
			CGameMaster.GM.GetMasterSignals().EmitSignal(CMasterSignals.SignalName.BenchmarkFinishPresset);
			GD.Print("SendSignal");
		}
	}

	public void AddFpsNow(){CGameMaster.GM.GetBenchmark().AddFpsData((int)Engine.GetFramesPerSecond());}

    public async void PostInit()
    {
        await ToSignal(CGameMaster.GM.GetMasterSignals(), CMasterSignals.SignalName.GameStart);
		GD.Print("GameStart receiverd");

		CGameMaster.GM.GetBenchmark().SetPostInitBenchmarkSettings();

		// start move benchmark camera
		updateMove = true;
    }
}
