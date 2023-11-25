using Godot;
using System;

public partial class CMasterSignals : Node
{
    // Game
    [Signal] public delegate void GameStartEventHandler();

    // Benchmark
    [Signal] public delegate void BenchmarkFinishPressetEventHandler();
    [Signal] public delegate void BenchmarkServerStatusEventHandler(bool newResult);

    public override void _Ready()
    {
        base._Ready();

        GameStart += BenchmarkGameStartEvent;
    }

    public void BenchmarkGameStartEvent()
    {
        //GD.Print("Master Signal - GameStart");
    }
}
