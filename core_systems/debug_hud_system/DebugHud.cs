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
		var callable_UpdateElements = new Callable(this, "UpdateTimer");
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
		if (Input.IsActionJustPressed("ToggleDebugHud"))
			SetOptionsEnable(!isOptionsPanelEnabled);
	}

	// Enable/Disahlbe DebugHud (on off include all updates)
	public void SetEnable(bool newEnable)
	{
		Visible = newEnable;
		isEnable = newEnable;

		if (newEnable)
		{
			update_timer.Start();
		}
		else
		{
			update_timer.Stop();
			SetOptionsEnable(false);
		}
	}

	public bool GetEnable() { return isEnable; }

	public void SetOptionsEnable(bool newEnable)
	{
		isOptionsPanelEnabled = newEnable;

		if (isOptionsPanelEnabled)
		{
			// PANEL ENABLED
			DebugEnabledLabel.Text = "F1 for hide these options";
			OptionsPanel.Visible = true;
			GameMaster.GM.GetFPSCharacter().SetInputEnable(false);
			GameMaster.GM.GetFPSCharacter().SetMouseVisible(true);
			Input.MouseMode = Input.MouseModeEnum.Confined;

			// updatuje vsechny controls prvky
			ApplyAllVideoControls();
			ApplyAllAudioControls();
		}
		else
		{
			// PANEL DISABLED
			DebugEnabledLabel.Text = "F1 for edit debug hud";
			OptionsPanel.Visible = false;
			GameMaster.GM.GetFPSCharacter().SetInputEnable(true);
			GameMaster.GM.GetFPSCharacter().SetMouseVisible(false);
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	// je spousten podle timeru
	private void UpdateTimer()
	{
		if (FPSLabel.Visible)
			FPSLabel.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();

		// pokud je performancePanel visible tak updatujeme labely
		if (PerformancePanel.Visible)
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
			CustomLabels[i].Text = "CL[" + i + "]";
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
	public void SetCustomLabelEnable(int idCustomLabel, bool newUpdateAndVisble)
	{
		CustomLabels[idCustomLabel].Visible = newUpdateAndVisble;

		GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO,
			"CustomLabel[" + idCustomLabel + "] set visible " + newUpdateAndVisble);
	}

	public bool GetCustomLabelEnable(int idCustomLabel)
	{
		return CustomLabels[idCustomLabel].Visible;
	}
	public void _on_show_fps_check_box_toggled(bool isPressed)
	{
		FPSLabel.Visible = isPressed;
	}


	public void _on_custom_label_enable_checkbox_toggled(bool isPressed, int id)
	{
		SetCustomLabelEnable(id, isPressed);
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
			level_button_Instance.SetLevelData(level.path, level.name);

			VBoxContainer LevelButtonContainer = GetNode<VBoxContainer>("OptionsPanel/TabContainer/level");
			LevelButtonContainer.AddChild(level_button_Instance);
		}
	}

	public void CheckScreenModeSetting()
	{
		// pokud mame vybrany mod windowed, tak povolime moznost vybrat velikost okna, jinak ne
		if(GameMaster.GM.GetSettings().GetActual_ScreenMode() == 0)
		{
            GetNode<OptionButton>("OptionsPanel/TabContainer/video/WindowSize_HBoxContainer/" +
                "WindowSize_OptionButton").Disabled = false;
			GetNode<OptionButton>("OptionsPanel/TabContainer/video/WindowSize_HBoxContainer/" +
				"WindowSize_OptionButton").Selected = GameMaster.GM.GetSettings().GetActual_ScreenSizeID();
        }
        else
		{
            GetNode<OptionButton>("OptionsPanel/TabContainer/video/WindowSize_HBoxContainer/" +
                "WindowSize_OptionButton").Disabled = true;
            GetNode<OptionButton>("OptionsPanel/TabContainer/video/WindowSize_HBoxContainer/" +
                "WindowSize_OptionButton").Selected = 2;
        }
    }

	// Signal pro zmenu screen mode skrze option button
    public void _on_screen_mode_option_button_item_selected(int newID)
	{
		GameMaster.GM.GetSettings().Apply_ScreenMode(newID,true,false);
		CheckScreenModeSetting();	// volame pro logiku zapnuti/vypnuti moznosti vybirat velikost okna
    }

    // Signal pro zmenu rozliseni skrze option button
    public void _on_window_size_option_button_item_selected(int newID)
    {
		// only apply
        GameMaster.GM.GetSettings().Apply_ScreenSizeID(newID, true, false);
    }

    // Signal pr zmenu antialiasingu skrze option button
    public void _on_antialias_option_button_item_selected(int newID)
	{
		// only apply
		GameMaster.GM.GetSettings().Apply_AntialiasID(newID, true, false);
	}

    public void _on_scale_3d_h_slider_value_changed(float newValue)
	{
        // only apply
        GameMaster.GM.GetSettings().Apply_Scale3D(newValue / 100.0f,true,false);

		Label scale3dLabel = GetNode<Label>("OptionsPanel/TabContainer/video/Scale3d_HBoxContainer/Scale3dvalue_Label");
		scale3dLabel.Text = (newValue / 100.0f).ToString();
    }

	public void _on_half_res_gi_check_box_toggled(bool newPressed)
	{
        // only apply
        GameMaster.GM.GetSettings().Apply_HalfResolutionGI(newPressed,true,false);
    }

	public void _on_ssao_check_box_toggled(bool newPressed)
	{
        // only apply
        GameMaster.GM.GetSettings().Apply_Ssao(newPressed,true,false);
    }

    public void _on_ssil_check_box_toggled(bool newPressed)
	{
        // only apply
		GameMaster.GM.GetSettings().Apply_Ssil(newPressed,true,false);
    }

    public void _on_sdfgi_check_box_toggled(bool newPressed)
	{
       // only apply
	   GameMaster.GM.GetSettings().Apply_Sdfgi(newPressed,true,false);
    }

    public void _on_unlock_max_fps_check_box_toggled(bool newPressed)
	{
		// only apply
		GameMaster.GM.GetSettings().Apply_UnlockMaxFps(newPressed,true,false);
	}

    public void _on_save_as_default_button_pressed()
	{
		// save all actual graphics settings
		GameMaster.GM.GetSettings().SaveActual_AllGraphicsSettings();
	}

	public void ApplyAllVideoControls()
	{
        // screen mode
        OptionButton screenmode_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"ScreenMode_HBoxContainer/ScreenMode_OptionButton");
		screenmode_option.Selected = GameMaster.GM.GetSettings().GetActual_ScreenMode();

        CheckScreenModeSetting();	// volame pro logiku zapnuti/vypnuti moznosti vybirat velikost okna

        // antialias 
        OptionButton antialias_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"Antialias_HBoxContainer/Antialias_OptionButton");
		antialias_option.Selected = GameMaster.GM.GetSettings().GetActual_AntialiasID();

		OptionButton windowsize_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"WindowSize_HBoxContainer/WindowSize_OptionButton");
		windowsize_option.Selected = GameMaster.GM.GetSettings().GetActual_ScreenSizeID();

        // scale 3d
        HSlider scale3d_slider = GetNode<HSlider>("OptionsPanel/TabContainer/video/" +
			"Scale3d_HBoxContainer/Scale3d_HSlider");
		scale3d_slider.Value = GameMaster.GM.GetSettings().GetActual_Scale3D() * 100.0f;

        Label scale3d_label = GetNode<Label>("OptionsPanel/TabContainer/video/Scale3d_HBoxContainer/Scale3dvalue_Label");
        scale3d_label.Text = GameMaster.GM.GetSettings().GetActual_Scale3D().ToString();

		// half resolution gi
		CheckBox halfresgi_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/HalfResGI_CheckBox");
		halfresgi_checkbox.ButtonPressed = GameMaster.GM.GetSettings().GetActual_HalfResolutionGI();

		// ssao
		CheckBox ssao_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/Ssao_CheckBox");
		ssao_checkbox.ButtonPressed = GameMaster.GM.GetSettings().GetActual_Ssao();

        // ssil
        CheckBox ssil_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/Ssil_CheckBox");
        ssil_checkbox.ButtonPressed = GameMaster.GM.GetSettings().GetActual_Ssil();

        // sdfgi
        CheckBox sdfgi_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/Sdfgi_CheckBox");
        sdfgi_checkbox.ButtonPressed = GameMaster.GM.GetSettings().GetActual_Sdfgi();

		CheckBox unlockmaxfps_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/UnlockMaxFps_CheckBox");
		unlockmaxfps_checkbox.ButtonPressed = GameMaster.GM.GetSettings().GetActual_UnlockMaxFps();
    }

    public void _on_main_volume_h_slider_value_changed(float newValue)
	{
        // only apply
        GameMaster.GM.GetSettings().Apply_MainVolume(newValue, true, false);

		// update label
        Label mainVolume_label = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_Label");
        mainVolume_label.Text = newValue.ToString() + " db"; ;
    }

	public void _on_sfx_volume_h_slider_value_changed(float newValue)
	{
        // only apply
        GameMaster.GM.GetSettings().Apply_SfxVolume(newValue, true, false);

        // update label
        Label sfxVolume_label = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer2/sfxVolume_Label");
        sfxVolume_label.Text = newValue.ToString() + " db"; ;
    }

	public void _on_music_volume_h_slider_value_changed(float newValue)
	{
        // only apply
        GameMaster.GM.GetSettings().Apply_MusicVolume(newValue, true, false);

        // update label
        Label musicVolume_label = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer3/musicVolume_Label");
        musicVolume_label.Text = newValue.ToString() + " db"; ;
    }

	public void ApplyAllAudioControls()
	{
		// main volume
		HSlider mainVolumeSlider = GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_HSlider");
		mainVolumeSlider.Value = GameMaster.GM.GetSettings().GetActual_MainVolume();

		Label mainVolumeLabel = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_Label");
		mainVolumeLabel.Text = GameMaster.GM.GetSettings().GetActual_MainVolume().ToString() + " db";

        // sfx volume
        HSlider sfxVolumeSlider = GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer2/sfxVolume_HSlider");
		sfxVolumeSlider.Value = GameMaster.GM.GetSettings().GetActual_SfxVolume();

        Label sfxVolumeLabel = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer2/sfxVolume_Label");
        sfxVolumeLabel.Text = GameMaster.GM.GetSettings().GetActual_SfxVolume().ToString() + " db"; ;

        // sfx volume
        HSlider musicVolumeSlider = GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer3/musicVolume_HSlider");
        musicVolumeSlider.Value = GameMaster.GM.GetSettings().GetActual_MusicVolume();

        Label musicVolumeLabel = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer3/musicVolume_Label");
        musicVolumeLabel.Text = GameMaster.GM.GetSettings().GetActual_MusicVolume().ToString() + " db"; ;
    }

	public void _on_save_audio_as_default_button_pressed()
	{
		// save all actual graphics settings
		GameMaster.GM.GetSettings().SaveActual_AllAudioSettings();
    }

	public void ApplyAllMainControls()
	{
		// ziskame ulozena data
		global_settings_data data = GameMaster.GM.GetSettings().GetData();

		SetEnable(data.ShowDebugHud);

        GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowFps_CheckBox").ButtonPressed = data.ShowFps;
        _on_show_performance_check_box_toggled(data.ShowPerformance);

        GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowPerformance_CheckBox").ButtonPressed = data.ShowPerformance;

		GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox0").ButtonPressed = data.ShowCustomLabel0;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox1").ButtonPressed = data.ShowCustomLabel1;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox2").ButtonPressed = data.ShowCustomLabel2;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox3").ButtonPressed = data.ShowCustomLabel3;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox4").ButtonPressed = data.ShowCustomLabel4;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox5").ButtonPressed = data.ShowCustomLabel5;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox6").ButtonPressed = data.ShowCustomLabel6;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox7").ButtonPressed = data.ShowCustomLabel7;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox8").ButtonPressed = data.ShowCustomLabel8;
        GetNode<CheckBox>("OptionsPanel/TabContainer/main/CustomLabelEnable_Checkbox9").ButtonPressed = data.ShowCustomLabel9;

        GetNode<CheckBox>("OptionsPanel/TabContainer/main/EnableWorldOcclusionCulling_CheckBox").ButtonPressed = 
			data.EnableWorldLevelOcclusionCull;
    }

	public void _on_save_main_as_default_button_pressed()
	{
        // ziskame ulozena data
        global_settings_data data = GameMaster.GM.GetSettings().GetData();

		data.ShowDebugHud = isEnable;
		data.ShowFps = GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowFps_CheckBox").ButtonPressed;
		data.ShowPerformance = GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowPerformance_CheckBox").ButtonPressed;

		data.ShowCustomLabel0 = GetCustomLabelEnable(0);
        data.ShowCustomLabel1 = GetCustomLabelEnable(1);
        data.ShowCustomLabel2 = GetCustomLabelEnable(2);
        data.ShowCustomLabel3 = GetCustomLabelEnable(3);
        data.ShowCustomLabel4 = GetCustomLabelEnable(4);
        data.ShowCustomLabel5 = GetCustomLabelEnable(5);
        data.ShowCustomLabel6 = GetCustomLabelEnable(6);
        data.ShowCustomLabel7 = GetCustomLabelEnable(7);
        data.ShowCustomLabel8 = GetCustomLabelEnable(8);
        data.ShowCustomLabel9 = GetCustomLabelEnable(9);

		data.EnableWorldLevelOcclusionCull =
			GetNode<CheckBox>("OptionsPanel/TabContainer/main/EnableWorldOcclusionCulling_CheckBox").ButtonPressed;

		// ulozime data
		data.Save();
    }

}
