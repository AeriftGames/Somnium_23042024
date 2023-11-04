using Godot;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static LevelDataSettings;

// spusti benchmark urciteho levelu - znamena to ze pusti benchlevel(lowest) pak (low) atd
// az probehnou vsechny quality settings daneho benchmark levelu, vrati se do main menu
//

public partial class CBenchmarkSystem : Node
{
    [ExportGroup("BenchmarkQuality")]
    [Export] public bool EnableLowest = true;   //0
    [Export] public bool EnableLow = true;      //1
    [Export] public bool EnableMedium = true;   //2
    [Export] public bool EnableHigh = true;     //3
    [Export] public bool EnableHighest = true;  //4

    private int ActualBenchmarkQualityLevel = -1;
    public int NeedBenchmarkQualityLevel = -1;
    public bool BenchmarkEnd = false;

    public override void _Ready()
    {
        base._Ready();
    }

    public int GetActualQualityBenchmark(){ return ActualBenchmarkQualityLevel; }

    public void StartBenchmarkLevel(string newLevelScenePath, string newLevelName)
    {
        // inicializace prvniho benchmarku - na jakem quality zacneme
        if (EnableLowest) NeedBenchmarkQualityLevel = 0;
        else if (EnableLow) NeedBenchmarkQualityLevel = 1;
        else if (EnableMedium) NeedBenchmarkQualityLevel = 2;
        else if (EnableHigh) NeedBenchmarkQualityLevel = 3;
        else if (EnableHighest) NeedBenchmarkQualityLevel = 4;

        ActualBenchmarkQualityLevel = NeedBenchmarkQualityLevel;

        // start
        LoadBenchmarLevelInQuality(newLevelScenePath, newLevelName, NeedBenchmarkQualityLevel);
    }

    public void BenchmarkStart(bool success)
    {
        GD.Print("Benchmark level start in quality presset: " + NeedBenchmarkQualityLevel);

        GameMaster.GM.GetLoadingHud().Visible = false;

        // priprava na dalsi test

        bool finding = true;
        while (finding)
        {
            NeedBenchmarkQualityLevel++;

            if (NeedBenchmarkQualityLevel == 0 && EnableLowest)
                finding = false;
            else if (NeedBenchmarkQualityLevel == 1 && EnableLow)
                finding = false;
            else if (NeedBenchmarkQualityLevel == 2 && EnableMedium)
                finding = false;
            else if (NeedBenchmarkQualityLevel == 3 && EnableHigh)
                finding = false;
            else if (NeedBenchmarkQualityLevel == 4 && EnableHighest)
                finding = false;
            else if (NeedBenchmarkQualityLevel > 4)
            {
                finding = false;
                BenchmarkEnd = true;
            }
        }
    }

    public void NextBenchmarkQuality()
    {
        //await Task.Delay(5000);

        LoadBenchmarLevelInQuality("res://levels/worldlevel_demo_extend_benchmark.tscn",
            "benchmark_level_1", NeedBenchmarkQualityLevel);
    }

    public async void LoadBenchmarLevelInQuality(string newLevelScenePath, string newLevelName,int newQualityPresset)
    {
        // normalni process pro (thread) nacteni noveho levelu vcetne loading baru atd
        GameMaster.GM.GetLevelLoader().LoadNewWorldLevel(newLevelScenePath, newLevelName);

        // ma jedinou funkci a pouzivame ji pro zobrazeni textu v benchmarku
        ActualBenchmarkQualityLevel = NeedBenchmarkQualityLevel;

        // pockame v teto asynchronni funkci na signal kdy se benchmark level uspesne nacetl
        await ToSignal(GameMaster.GM.GetLevelLoader(), CLevelLoader.SignalName.LevelLoadComplete);
        GD.Print("Benchmark level Load Complete");

        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Apply nastaveni quality pressetu
        GameMaster.GM.GetLevelLoader().GetActualLevelScene().GetLevelDataSettings().
            ApplyNewLevelDataSettings(GameMaster.GM.GetLevelLoader().
            GetActualLevelScene().GetLevelDataSettings().GetQualityPressetByID(NeedBenchmarkQualityLevel),
            false,true);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("TestStartBenchmarkLevel"))
            StartBenchmarkLevel("res://levels/worldlevel_demo_extend_benchmark.tscn", "benchmark_level_1");
    }
}
