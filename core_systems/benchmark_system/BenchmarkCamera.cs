using Godot;
using System;
using System.Threading.Tasks;

public partial class BenchmarkCamera : Camera3D
{
    Label FPSLabel;
    Label QualityLabel;

    public override void _Ready()
    {
        base._Ready();

        FPSLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer/FPSLabel");
        QualityLabel = GetNode<Label>("Control/VBoxContainer/HBoxContainer3/QualityText");

        PostInit();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        FPSLabel.Text = Engine.GetFramesPerSecond().ToString();
    }

    public async void PostInit()
    {
        await ToSignal(GameMaster.GM.GetLevelLoader(), CLevelLoader.SignalName.LevelLoadComplete);
        GD.Print("TARLALALALALALA");

        await Task.Delay(1000);

        // vyber textu aktualni kvality
        switch (GameMaster.GM.GetBenchmarkSystem().GetActualQualityBenchmark())
        {
            case 0: QualityLabel.Text = "lowest quality"; break;
            case 1: QualityLabel.Text = "low quality"; break;
            case 2: QualityLabel.Text = "medium quality"; break;
            case 3: QualityLabel.Text = "high quality"; break;
            case 4: QualityLabel.Text = "highest quality"; break;
        }

        GameMaster.GM.GetSettings().Apply_UnlockMaxFps(true, true);
        GameMaster.GM.GetSettings().Apply_DisableVsync(true, true);
    }
}
