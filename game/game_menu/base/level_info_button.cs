using Godot;
using System;

public partial class level_info_button : Button
{
    private levelinfo_base_resource Levelinfo = null;

    public void SetLevelInfo(levelinfo_base_resource newLevelInfo) { Levelinfo = newLevelInfo; }

    public void _on_pressed()
    {
        if (Levelinfo != null)
            GameMaster.GM.GetLevelLoader().LoadNewWorldLevel(Levelinfo.LevelPath, Levelinfo.LevelName);
    }
}
