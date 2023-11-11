using Godot;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public partial class benchmark_main_menu : Control
{
    private Label BuildLabel;
    private Control ServerOffControl;

    public override void _Ready()
    {
        base._Ready();

        BuildLabel = GetNode<Label>("VBoxContainer/Panel2/BuildLabel");
        ServerOffControl = GetNode<Control>("ServerOffControl");

        BuildLabel.Text = "build:"+GameMaster.GM.GetBuild();


        ServerOffControl.Visible = false;

        PostInit();
    }

    public void PostInit()
    {
        GameMaster.GM.GetMasterSignals().BenchmarkServerStatus += Benchmark_main_menu_BenchmarkServerStatus;
    }

    public void _on_start_benchmark_button_pressed()
    {
        //GameMaster.GM.GetBenchmarkSystem().ServerCheck_End(false);
        GameMaster.GM.GetBenchmarkSystem().ServerCheck();
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
}
