using Godot;
using System;

public partial class DebugHud : Control
{
	Panel OptionsPanel = null;
	Label DebugEnabledLabel = null;
	Label FPSLabel = null;

	bool isOptionsPanelEnabled = false;
    Godot.Timer update_timer = new Godot.Timer();

	CheckBox ShowFpsCheckBox = null;

    public override void _Ready()
	{
		OptionsPanel = GetNode<Panel>("OptionsPanel");
        DebugEnabledLabel = GetNode<Label>("DebugEnabledLabel");
        FPSLabel = GetNode<Label>("FPSLabel");

        ShowFpsCheckBox = GetNode<CheckBox>("OptionsPanel/VBoxContainer/ShowFps_CheckBox");

        OptionsPanel.Visible = false;

		//
        FPSLabel.Visible = true;
		ShowFpsCheckBox.ButtonPressed = true;


        DebugEnabledLabel.Text = "F1 for edit debug hud";

        // Create timer for specific loop update
        var callable_UpdateElements = new Callable(UpdateElements);
        update_timer.Connect("timeout", callable_UpdateElements);
        update_timer.WaitTime = 0.2;
        update_timer.OneShot = false;
        AddChild(update_timer);
		update_timer.Start();
    }

	public override void _Process(double delta)
	{
		// vypne/zapne tento debug
		if(Input.IsActionJustPressed("ToggleDebugHud"))
			SetEnable(!isOptionsPanelEnabled);
	}

	public void SetEnable(bool newEnable)
	{
		isOptionsPanelEnabled = newEnable;

		if(isOptionsPanelEnabled)
		{
            // PANEL ENABLED
            DebugEnabledLabel.Text = "F1 for hide these options";
			OptionsPanel.Visible = true;
			GameMaster.GM.GetFPSCharacter().SetInputEnable(false);
			Input.MouseMode = Input.MouseModeEnum.Confined;
        }
		else
		{
			// PANEL DISABLED
			DebugEnabledLabel.Text = "F1 for edit debug hud";
            OptionsPanel.Visible = false;
            GameMaster.GM.GetFPSCharacter().SetInputEnable(true);
            //GameMaster.GM.GetFPSCharacter().SetMouseVisible(false);
        }
	}

	private void UpdateElements()
	{
        FPSLabel.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
    }

	public void ShowElements(bool newShow)
	{

	}

	public void ApplyAllOptions()
	{

	}

    // Prepnuti checkboxu show fps
    public void _on_show_fps_check_box_pressed()
	{
		if(ShowFpsCheckBox.ButtonPressed)
		{
			FPSLabel.Visible = true;
		}
		else
		{
            FPSLabel.Visible = false;
        }
	}
}
