using Godot;
using System;

public partial class level_info_button : Button
{
    private levelinfo_base_resource Levelinfo = null;

    public void SetLevelInfo(levelinfo_base_resource newLevelInfo) { Levelinfo = newLevelInfo; }

    public void _on_pressed()
    {
        if (Levelinfo == null) return;
        /*if (OS.HasFeature("editor"))
        {*/
            // EDITOR
            if (Levelinfo.LevelType == WorldLevel.ELevelType.GameLevel)
                CGameMaster.GM.GetGame().GetLevelLoader().LoadNewWorldLevel(Levelinfo.LevelPath, Levelinfo.LevelName);
            else if (Levelinfo.LevelType == WorldLevel.ELevelType.BenchmarkLevel)
                CGameMaster.GM.GetBenchmark().StartBenchmarkLevel(Levelinfo.LevelPath, Levelinfo.LevelName);
            else if (Levelinfo.LevelType == WorldLevel.ELevelType.Menu)
                GetTree().ChangeSceneToFile(Levelinfo.LevelPath);
        /*}
        else
        {
            // EXPORT
            if (Levelinfo.LevelType == WorldLevel.ELevelType.GameLevel)
                GameMaster.GM.GetLevelLoader().LoadNewWorldLevel(Levelinfo.LevelPath + ".remap", Levelinfo.LevelName);
            else if (Levelinfo.LevelType == WorldLevel.ELevelType.BenchmarkLevel)
                GameMaster.GM.GetBenchmarkSystem().StartBenchmarkLevel(Levelinfo.LevelPath + ".remap", Levelinfo.LevelName);
            else if (Levelinfo.LevelType == WorldLevel.ELevelType.Menu)
                GetTree().ChangeSceneToFile(Levelinfo.LevelPath+".remap");
        }*/
    }
}
