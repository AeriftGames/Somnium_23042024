using Godot;
using System.Threading.Tasks;

public partial class MainApp : Node
{
    public enum EAppType { Game, Benchmark};
    [Export] public EAppType AppType = EAppType.Game;
    [ExportGroup("Start Scenes")]
    [Export] Resource LevelInfo_StartGame;
    [Export] Resource LevelInfo_StartBenchmark;
    
    public override void _Ready()
    {
        base._Ready();
        PostInit();
    }

    public async void PostInit()
    {
        await Task.Delay(1200);

        // Podle App Type
        switch (AppType)
        {
            case EAppType.Game:
                StartAsGame(); break;
            case EAppType.Benchmark:
                StartAsBenchmark(); break;
            default: break;
        }
    }

    public void StartAsGame()
    {
        levelinfo_base_resource LevelInfoData = UniversalFunctions.GetLevelInfoData(LevelInfo_StartGame);
        //GameMaster.GM.GetLevelLoader().LoadNewWorldLevel_Threaded(LevelInfoData.LevelPath,LevelInfoData.LevelName);
        GetTree().ChangeSceneToFile(LevelInfoData.LevelPath);
    }

    public void StartAsBenchmark()
    {
        levelinfo_base_resource LevelInfoData = UniversalFunctions.GetLevelInfoData(LevelInfo_StartBenchmark);
        GetTree().ChangeSceneToFile(LevelInfoData.LevelPath);
    }
}
