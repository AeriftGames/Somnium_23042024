using Godot;
using System;

public partial class level_info_button : Button
{
    private levelinfo_base_resource Levelinfo = null;

    public void SetLevelInfo(levelinfo_base_resource newLevelInfo) { Levelinfo = newLevelInfo; }

    public void _on_pressed()
    {
        if (Levelinfo == null) return;

        if (Levelinfo.LevelType == WorldLevel.ELevelType.GameLevel)
            GameMaster.GM.GetLevelLoader().LoadNewWorldLevel(Levelinfo.LevelPath, Levelinfo.LevelName);
        else if (Levelinfo.LevelType == WorldLevel.ELevelType.BenchmarkLevel)
            GameMaster.GM.GetBenchmarkSystem().StartBenchmarkLevel(Levelinfo.LevelPath, Levelinfo.LevelName);
        else if (Levelinfo.LevelType == WorldLevel.ELevelType.Menu)
            GetTree().ChangeSceneToFile(Levelinfo.LevelPath);
    }
}
