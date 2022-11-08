using Godot;
using Godot.Collections;
using System;
using System.Threading;

public partial class DebugHud : Control
{
	Panel OptionsPanel = null;
	Label DebugEnabledLabel = null;
	Label FPSLabel = null;
	Label[] CustomLabels = new Label[10];
	Panel PerformancePanel = null;

	bool isOptionsPanelEnabled = false;
    Godot.Timer update_timer = new Godot.Timer();

	CheckBox ShowFpsCheckBox = null;

	bool isEnable = false;

    public override void _Ready()
	{
        // pro dostupnost skrze gamemastera
        GameMaster.GM.SetDebugHud(this);

        OptionsPanel = GetNode<Panel>("OptionsPanel");
        DebugEnabledLabel = GetNode<Label>("DebugEnabledLabel");
        FPSLabel = GetNode<Label>("FPSLabel");
		PerformancePanel = GetNode<Panel>("PerformancePanel");

		ShowFpsCheckBox = GetNode<CheckBox>("OptionsPanel//TabContainer/main/ShowFps_CheckBox");

		InitAllCustomLabels();

        OptionsPanel.Visible = false;
		PerformancePanel.Visible = false;
        FPSLabel.Visible = true;
		ShowFpsCheckBox.ButtonPressed = true;
        DebugEnabledLabel.Text = "F1 for edit debug hud";

        // Create timer for specific loop update (fps)
        var callable_UpdateElements = new Callable(UpdateTimer);
        update_timer.Connect("timeout", callable_UpdateElements);
        update_timer.WaitTime = 0.2;
        update_timer.OneShot = false;
        AddChild(update_timer);
		update_timer.Stop();

		// On Default = for default visble..
		SetEnable(true);

		BuildLevelButtons();
    }

	public override void _Process(double delta)
	{
		if (isEnable == false) return;

		// vypne/zapne tento debug
		if(Input.IsActionJustPressed("ToggleDebugHud"))
			SetOptionsEnable(!isOptionsPanelEnabled);
	}

	// Enable/Disahlbe DebugHud (on off include all updates)
	public void SetEnable(bool newEnable)
	{
		Visible = newEnable;
		isEnable = newEnable;

		if(newEnable)
		{
            update_timer.Start();
        }
		else
		{
            update_timer.Stop();
			SetOptionsEnable(false);
        }
    }

	public void SetOptionsEnable(bool newEnable)
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

	// je spousten podle timeru
	private void UpdateTimer()
	{
		if(FPSLabel.Visible)
			FPSLabel.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();

		// pokud je performancePanel visible tak updatujeme labely
		if(PerformancePanel.Visible)
		{
			// draw calls
			GetNode<Label>("PerformancePanel/VBoxContainer/DrawCallsLabel").Text =
				"draw calls: " + Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);

            // draw objects
            GetNode<Label>("PerformancePanel/VBoxContainer/ObjectsLabel").Text =
                "objects: " + Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame);

            // draw primitives
            GetNode<Label>("PerformancePanel/VBoxContainer/PrimitivesLabel").Text =
                "primitives: " + Performance.GetMonitor(Performance.Monitor.RenderTotalPrimitivesInFrame);

            // draw primitives
            GetNode<Label>("PerformancePanel/VBoxContainer/VideoMemoryLabel").Text =
                "video memory used: " + Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed);
        }
    }

	public void InitAllCustomLabels()
	{
		VBoxContainer vbox = GetNode<VBoxContainer>("CustomLabelsVBox");
		for (int i = 0; i < vbox.GetChildCount(); i++)
		{
			// Projedeme vsechny prvky v CustomLabelVBox a ulozime si je do array CustomLabels
			CustomLabels[i] = (Label)vbox.GetChild(i);
			// Popiseme vsechny prvky
			CustomLabels[i].Text = "CL["+ i +"]";
		}
	}

    public void CustomLabelUpdateText(int idCustomLabel, Node newCallNode, string newText)
    {

		if (isEnable == false) return;

		// Pokud nekdo vola update textu, zjistime jestli zadane id je v rozsahu
        if (idCustomLabel >= 0 && idCustomLabel < (CustomLabels.Length - 1))
        {
			// je pokud je dany CustomLabel viditelny, updatujeme jeho text
			if (CustomLabels[idCustomLabel].Visible == true)
				CustomLabels[idCustomLabel].Text = "CL[" + idCustomLabel + "] (" + newCallNode.Name + ") " + newText;
        }
    }

    // Nastavi viditelnost a zaroven i moznost updatu daneho labelu
    public void SetCustomLabelUpdateAndVisible(int idCustomLabel, bool newUpdateAndVisble)
	{
		CustomLabels[idCustomLabel].Visible = newUpdateAndVisble;

        GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, 
			"CustomLabel[" + idCustomLabel + "] set visible " + newUpdateAndVisble);
    }
	// *** SIGNAL FROM ALL CHECKBOXES(CUSTOM LABELS) IN OPTIONSPANEL ***

	public void _on_show_fps_check_box_toggled(bool isPressed)
	{
		FPSLabel.Visible = isPressed;
	}


    public void _on_custom_label_enable_checkbox_toggled(bool isPressed, int id)
	{
		SetCustomLabelUpdateAndVisible(id,isPressed);
	}

    public void _on_enable_world_occlusion_culling_check_box_toggled(bool isPressed)
	{
		Node3D worldlevel_occluderculling = (Node3D)GetNode("/root/worldlevel/worldlevel_occluderculling");
		worldlevel_occluderculling.Visible = isPressed;

		GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO,
			"worldlevel_occluderculling set visible to: " + isPressed);
    }

	public void _on_show_performance_check_box_toggled(bool isPressed)
	{
		PerformancePanel.Visible = isPressed;
	}

	public void _on_quit_game_button_pressed()
	{
		GameMaster.GM.QuitGame();
	}


    public void BuildLevelButtons()
	{
		var allLevelsInfo = GameMaster.GM.LevelLoader.GetAllLevelsInfo();
		foreach (var level in allLevelsInfo)
		{
            // Instance Button
            level_button level_button_Instance = (level_button)GD.Load<PackedScene>(
                "res://core_systems/debug_hud_system/level_button.tscn").Instantiate();

            level_button_Instance.Text = level.name;
			level_button_Instance.SetLevelData(level.path,level.name);

			VBoxContainer LevelButtonContainer = GetNode<VBoxContainer>("OptionsPanel/TabContainer/level");
			LevelButtonContainer.AddChild(level_button_Instance);
        }
	}
}
