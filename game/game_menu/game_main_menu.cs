using Godot;
using System;
using System.Collections.Generic;

public partial class game_main_menu : Control
{
    Control PickLevelsControl;
    VBoxContainer VBoxContainer_Levels;

    public override void _Ready()
    {
        base._Ready();

        PickLevelsControl = GetNode<Control>("PickLevelsControl");
        VBoxContainer_Levels = GetNode<VBoxContainer>("PickLevelsControl/Panel2/Panel3/VBoxContainer");

        PickLevelsControl.Visible = false;
        LoadAllLevelsAsButtons();

        // Set visible mouse for navigate
        Input.MouseMode = Input.MouseModeEnum.Visible;

        GetNode<Button>("%StartLevelButton").GrabFocus();
    }

    public void LoadAllLevelsAsButtons()
    {
        PackedScene newLevelInfoButton = 
            GD.Load<PackedScene>("res://game/base/level_info_button.tscn");

        List<levelinfo_base_resource> AllLevelInfos = 
            UniversalFunctions.GetAllLevelInfoDataFromDir("res://levels/all_levels_info_resources/game_levels/");

        foreach (levelinfo_base_resource levelinfo in AllLevelInfos)
        {
            GD.Print(levelinfo.LevelPath);
            level_info_button b = newLevelInfoButton.Instantiate<level_info_button>();
            b.Text = levelinfo.LevelName;
            VBoxContainer_Levels.AddChild(b);
            b.SetLevelInfo(levelinfo);
        }
    }

    public void _on_start_level_button_pressed()
    {
        PickLevelsControl.Visible = true;
        GetNode<VBoxContainer>("%VBoxContainer").Visible = false;
        GetNode<Button>("%ClosePickLevelsControl").GrabFocus();
    }

    public void _on_close_pick_levels_control_pressed()
    {
        PickLevelsControl.Visible = false;
        GetNode<VBoxContainer>("%VBoxContainer").Visible = true;
        GetNode<Button>("%StartLevelButton").GrabFocus();
    }

    public void _on_exit_game_menu_pressed()
    {
        GetTree().Quit();
    }
}
