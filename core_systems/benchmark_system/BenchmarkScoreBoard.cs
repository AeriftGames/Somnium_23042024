using Godot;
using System;

public partial class BenchmarkScoreBoard : Control
{
    public Label BuildLabel;
    public Label LevelNameLabel;
    public Label QualityLabel;
    public Label MinFpsLabel;
    public Label MaxFpsLabel;
    public Label AvgFpsLabel;

    public override void _Ready()
    {
        base._Ready();

        BuildLabel = GetNode<Label>("%BuildLabel");
        LevelNameLabel = GetNode<Label>("%LevelNameLabel");
        QualityLabel = GetNode<Label>("%QualityLabel");
        MinFpsLabel = GetNode<Label>("%MinFpsLabel");
        MaxFpsLabel = GetNode<Label>("%MaxFpsLabel");
        AvgFpsLabel = GetNode<Label>("%AvgFpsLabel");

        SetVisibleForPlayer(false);
    }

    public void SetBenchmarkScoreData(string newBuild, string newLevelName,string newQualityLevel, string newFpsAvg,
        string newFpsMin, string newFpsMax)
    {
        BuildLabel.Text = "build: " + newBuild;
        LevelNameLabel.Text = "levelname: " + newLevelName;
        QualityLabel.Text = "quality: " + newQualityLevel;
        MinFpsLabel.Text = "min fps: " +newFpsMin;
        MaxFpsLabel.Text = "max fps: " + newFpsMax;
        AvgFpsLabel.Text = "avg fps: " + newFpsAvg;
    }

    public void SetVisibleForPlayer(bool newVisible){Visible = newVisible;}
}
