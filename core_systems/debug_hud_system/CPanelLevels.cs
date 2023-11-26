using Godot;
using System;
using System.Collections.Generic;

public partial class CPanelLevels : CPanelBase
{
    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);
        BuildLevelButtons();
    }

    public void BuildLevelButtons()
    {
        PackedScene newLevelInfoButton =
            GD.Load<PackedScene>("res://core_systems/debug_hud_system/level_info_button1.tscn");

        List<levelinfo_base_resource> AllLevelInfos =
            UniversalFunctions.GetAllLevelInfoDataFromDir("res://levels/all_levels_info_resources/game_levels/");

        foreach (levelinfo_base_resource levelinfo in AllLevelInfos)
        {
            GD.Print(levelinfo.LevelPath);
            level_info_button b = newLevelInfoButton.Instantiate<level_info_button>();
            b.Text = levelinfo.LevelName;

            VBoxContainer LevelButtonContainer = GetVBoxElements();
            LevelButtonContainer.AddChild(b);
            b.SetLevelInfo(levelinfo);
        }

    }
}
