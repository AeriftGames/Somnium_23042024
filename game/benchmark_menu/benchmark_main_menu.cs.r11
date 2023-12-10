using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public partial class benchmark_main_menu : Control
{
    private Label BuildLabel;
    private Control PickLevelsControl;
    private VBoxContainer VBoxContainer_Levels;
    private Control ServerOffControl;

    public override void _Ready()
    {
        base._Ready();

        BuildLabel = GetNode<Label>("VBoxContainer/Panel2/BuildLabel");
        PickLevelsControl = GetNode<Control>("PickLevelsControl");
        VBoxContainer_Levels = GetNode<VBoxContainer>("PickLevelsControl/Panel2/Panel3/VBoxContainer");
        ServerOffControl = GetNode<Control>("ServerOffControl");

        BuildLabel.Text = "build:"+GameMaster.GM.GetBuild();


        ServerOffControl.Visible = false;
        PickLevelsControl.Visible = false;
        LoadAllLevelsAsButtons();

        PostInit();
    }
    public void LoadAllLevelsAsButtons()
    {
        PackedScene newLevelInfoButton =
            GD.Load<PackedScene>("res://game/base/level_info_button.tscn");

        List<levelinfo_base_resource> AllLevelInfos =
            UniversalFunctions.GetAllLevelInfoDataFromDir("res://levels/all_levels_info_resources/benchmark_levels/");

        foreach (levelinfo_base_resource levelinfo in AllLevelInfos)
        {
            GD.Print(levelinfo.LevelPath);
            level_info_button b = newLevelInfoButton.Instantiate<level_info_button>();
            b.Text = levelinfo.LevelName;
            VBoxContainer_Levels.AddChild(b);
            b.SetLevelInfo(levelinfo);
        }
    }

    public void PostInit()
    {
        GameMaster.GM.GetMasterSignals().BenchmarkServerStatus += Benchmark_main_menu_BenchmarkServerStatus;
    }

    public void _on_start_benchmark_button_pressed()
    {
        //GameMaster.GM.GetBenchmarkSystem().ServerCheck_End(false);
        //GameMaster.GM.GetBenchmarkSystem().ServerCheck();
        PickLevelsControl.Visible = true;
    }

    private void Benchmark_main_menu_BenchmarkServerStatus(bool newResult)
    {
        if(newResult)
        {
            StartNewBenchmark();
        }
        else
        {
            ServerOffControl.Visible = true;
        }
    }

    public void StartNewBenchmark()
    {
        GameMaster.GM.GetBenchmarkSystem().StartBenchmarkLevel(
            "res://levels/worldlevel_demo_extend_benchmark.tscn", "BenchmarkLevel1");
    }

    public void _on_exit_benchmark_button_pressed()
    {
        GetTree().Quit();
    }

    public void _on_close_server_off_benchmark_button_pressed()
    {
        ServerOffControl.Visible = false;
    }

    public void _on_start_now_benchmark_button_pressed()
    {
        StartNewBenchmark();
    }

    public void _on_close_pick_levels_control_pressed()
    {
        PickLevelsControl.Visible = false;
    }
}
