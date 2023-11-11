using Godot;
using System;

public partial class MainApp : Node
{
    public override void _Ready()
    {
        base._Ready();

        // Podle Game Type
        switch (GameMaster.GM.AppType)
        {
            case GameMaster.EAppType.Game:
                StartAsGame();
                break;
            case GameMaster.EAppType.Benchmark:
                StartAsBenchmark();
                break;
            default:
                break;
        }
    }

    public void StartAsGame()
    {
        //PackedScene StartGamePackedScene
        //GameMaster.GM.GetTree().ChangeSceneToPacked(StartGamePackedScene);
    }

    public void StartAsBenchmark()
    {

    }
}
