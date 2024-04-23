using Godot;
using System;
using System.Dynamic;
using System.Threading.Tasks;

public partial class BenchmarkMenuAndFpsStats : Control
{
    Label FPSLabel;
    Label QualityLabel;
    InBenchmarkMenu inBenchmarkMenu;

    public override void _Ready()
    {
        base._Ready();

        FPSLabel = GetNode<Label>("VBoxContainer/HBoxContainer/FPSLabel");
        QualityLabel = GetNode<Label>("VBoxContainer/HBoxContainer3/QualityText");
        inBenchmarkMenu = GetNode<InBenchmarkMenu>("InBenchmarkMenu");

        Visible = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        FPSLabel.Text = Engine.GetFramesPerSecond().ToString();

        if (Input.IsActionJustPressed("EscapeAction"))
            inBenchmarkMenu.SetActive(!inBenchmarkMenu.GetActive());
    }

    public InBenchmarkMenu GetBenchmarkMenu() { return  inBenchmarkMenu; }

    public void SetQualityLevelText(string newQualityLevelText)
    {
        QualityLabel.Text = newQualityLevelText;
    }
}
