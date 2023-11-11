using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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

    [ExportGroup("BenchmarkOther")]
    [Export] public float AddFpsDataTimer = 1.0f;

    private int ActualBenchmarkQualityLevel = -1;
    public int NeedBenchmarkQualityLevel = -1;
    public bool BenchmarkEnd = false;

    //
    private Array<int> AllFpsData = new Array<int>();

    private BenchmarkScoreBoard benchmarkScoreBoard;
    private BenchmarkMenuAndFpsStats benchmarkMenuAndFpsStats;

    private Node BenchmarkGD;

    public override void _Ready()
    {
        base._Ready();

        benchmarkScoreBoard = GetNode<BenchmarkScoreBoard>("BenchmarkScoreBoard");
        benchmarkMenuAndFpsStats = GetNode<BenchmarkMenuAndFpsStats>("BenchmarkMenuAndFpsStats");

        BenchmarkGD = GetNode<Node>("Benchmark");

        PostReady();
    }

    public async void PostReady()
    {
        //await ToSignal(GameMaster.GM.GetMasterSignals(), MasterSignals.SignalName.GameStart);
        await Task.Delay(1000);

        GameMaster.GM.GetMasterSignals().BenchmarkFinishPresset += () => FinishBenchmarkPresset();
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

        if (GameMaster.GM.GetFPSCharacter != null)
            GameMaster.GM.QueueCharacterAndCamera();
    }

    public void BenchmarkStart(bool success)
    {
        //GD.Print("Benchmark level start in quality presset: " + NeedBenchmarkQualityLevel);
        //await Task.Delay(1000);
        // Emit start game from now
        GameMaster.GM.GetMasterSignals().EmitSignal(MasterSignals.SignalName.GameStart);

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

    public async void NextBenchmarkQuality()
    {
        await Task.Delay(5000);

        benchmarkScoreBoard.SetVisibleForPlayer(false);
        AllFpsData.Clear();

        LoadBenchmarLevelInQuality(GameMaster.GM.GetLevelLoader().GetActualLevelScene().SceneFilePath,
            GameMaster.GM.GetLevelLoader().GetActualLevelName(), NeedBenchmarkQualityLevel);
    }

    public async void LoadBenchmarLevelInQuality(string newLevelScenePath,
        string newLevelName,int newQualityPresset)
    {
        // normalni process pro (thread) nacteni noveho levelu vcetne loading baru atd
        GameMaster.GM.GetLevelLoader().LoadNewWorldLevel(newLevelScenePath, newLevelName);

        // ma jedinou funkci a pouzivame ji pro zobrazeni textu v benchmarku
        ActualBenchmarkQualityLevel = NeedBenchmarkQualityLevel;

        // pockame v teto asynchronni funkci na signal kdy se benchmark level uspesne nacetl
        await ToSignal(GameMaster.GM.GetLevelLoader(), CLevelLoader.SignalName.LevelLoadComplete);
        //GD.Print("Benchmark level Load Complete");

        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Apply nastaveni quality pressetu
        GameMaster.GM.GetLevelLoader().GetActualLevelScene().GetLevelDataSettings().
            ApplyNewLevelDataSettings(GameMaster.GM.GetLevelLoader().
            GetActualLevelScene().GetLevelDataSettings().GetQualityPressetByID(NeedBenchmarkQualityLevel),
            false,true);
    }

    public async void FinishBenchmarkPresset()
    {
        // spocitame info fps - avg min max
        UniversalFunctions.SFpsInfo FpsInfo = UniversalFunctions.CalculateFpsInfo(AllFpsData);

        // vypsani vysledku ve scoreboard klientovi
        benchmarkScoreBoard.SetBenchmarkScoreData(GameMaster.GM.GetBuild(),
            GameMaster.GM.GetLevelLoader().GetActualLevelName(),
            UniversalFunctions.GetQualityLevelText(ActualBenchmarkQualityLevel),
            FpsInfo.avgFps.ToString(), FpsInfo.minFps.ToString(), FpsInfo.maxFps.ToString());
        benchmarkScoreBoard.SetVisibleForPlayer(true);

        // zkouska poslani dat oalovi
        SendBenchmarkScoreData(GameMaster.GM.GetBuild(),
            GameMaster.GM.GetLevelLoader().GetActualLevelName(),
            FpsInfo.avgFps, FpsInfo.minFps, FpsInfo.maxFps);

        //
        BenchmarkGD.Call("send_test", GameMaster.GM.GetBuild(), GameMaster.GM.GetLevelLoader().GetActualLevelName(),
            UniversalFunctions.GetQualityLevelText(ActualBenchmarkQualityLevel), FpsInfo.minFps, FpsInfo.maxFps,
            FpsInfo.avgFps);
    }

    public void FinishBenchmarkPresset_End(int newResult)
    {
        GD.Print("vse ok");

        if (BenchmarkEnd == true)
        {

        }
        else
        {
            NextBenchmarkQuality();
        }
    }

    public void SetPostInitBenchmarkSettings()
    {
        benchmarkMenuAndFpsStats.SetQualityLevelText(
            UniversalFunctions.GetQualityLevelText(ActualBenchmarkQualityLevel));

        GameMaster.GM.GetSettings().Apply_UnlockMaxFps(true, true);
        GameMaster.GM.GetSettings().Apply_DisableVsync(true, true);

        benchmarkMenuAndFpsStats.Visible = true;
    }

public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void SendBenchmarkScoreData(string newBuild,string newLevelName,int newFpsAvg,int newFpsMin,int newFpsMax)
    {
    }

    // FPS
    public void AddFpsData(int newFpsData)
    {
        AllFpsData.Add(newFpsData);
    }
}
