using Godot;
using System;

public partial class MasterSignals : Node
{
    [Signal] public delegate void GameStartEventHandler();
    [Signal] public delegate void BenchmarkFinishPressetEventHandler();
}
