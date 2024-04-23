using Godot;
using System.Threading.Tasks;

public partial class MainApp : Node
{
    public enum EAppType { Game,Benchmark,SpecificLevel };
    [Export] public EAppType AppType = EAppType.Game;
    [ExportGroup("Start specific Level")]
    levelinfo_base_resource LevelInfo_StartGame;
    levelinfo_base_resource LevelInfo_StartBenchmark;
    [Export] Resource LevelInfo_SpecificLevelInfo = null;

    public override void _Ready()
    {
        base._Ready();
        PostInit();
    }

    public async void PostInit()
    {
        LevelInfo_StartGame = UniversalFunctions.GetLevelInfoData(
            GD.Load("res://levels/all_levels_info_resources/menus/levelinfo_game_menu.tres"));

        LevelInfo_StartBenchmark = UniversalFunctions.GetLevelInfoData(
            GD.Load("res://levels/all_levels_info_resources/menus/levelinfo_benchmark_menu.tres"));

        await Task.Delay(1200);

        // Podle App Type
        switch (AppType)
        {
            case EAppType.Game:
                RunLevelInfo(LevelInfo_StartGame); break;
            case EAppType.Benchmark:
                RunLevelInfo(LevelInfo_StartBenchmark); break;
            case EAppType.SpecificLevel:
                RunLevelInfo(UniversalFunctions.GetLevelInfoData(LevelInfo_SpecificLevelInfo)); break;
            default: break;
        }
    }

    public void RunLevelInfo(levelinfo_base_resource newLevelInfo)
    {
        if (newLevelInfo == null) { GD.Print("(CHYBA) MainApp RunLevelInfo - newLevelInfo = null"); return; }

        CGameMaster.GM.GetUniversal().EnableBlackScreen(true);

        if (newLevelInfo.LevelType == WorldLevel.ELevelType.GameLevel)
            CGameMaster.GM.GetGame().GetLevelLoader().LoadNewWorldLevel(newLevelInfo.LevelPath, newLevelInfo.LevelName);
        else if (newLevelInfo.LevelType == WorldLevel.ELevelType.BenchmarkLevel)
            CGameMaster.GM.GetBenchmark().StartBenchmarkLevel(newLevelInfo.LevelPath, newLevelInfo.LevelName);
        else if (newLevelInfo.LevelType == WorldLevel.ELevelType.Menu)
            GetTree().ChangeSceneToFile(newLevelInfo.LevelPath);
    }
}
    