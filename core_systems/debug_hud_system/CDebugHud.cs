using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class CDebugHud : Control
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

	public void PostInit()
	{
        // pro dostupnost skrze gamemastera
        CGameMaster.GM.SetDebugHud(this);

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
		/*
        // On Default = for default visble..
        SetEnable(true);

        BuildLevelButtons();
        BuildSpawnButtons();*/
    }

	public override void _Process(double delta)
	{
		/*
		if (isEnable == false) return;

		// vypne/zapne tento debug
		if (Input.IsActionJustPressed("ToggleDebugHud"))
			SetOptionsEnable(!isOptionsPanelEnabled);

        // prepne hud page vpred
        if (Input.IsActionJustPressed("NextPageInHud"))
			SetCurrentTab(GetCurrentTabID() + 1, true);

		// prepne hud page vzad
		if(Input.IsActionJustPressed("PreviousPageInHud"))
            SetCurrentTab(GetCurrentTabID() - 1, true);*/
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

			if(CGameMaster.GM.GetGame().GetFPSCharacterOld() != null)
			{
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetInputEnable(false);
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetMouseVisible(true);
                Input.MouseMode = Input.MouseModeEnum.Confined;
            }

			// updatuje vsechny controls prvky
			ApplyAllVideoControls();
			ApplyAllAudioControls();
			ApplyAllInputsControls();

			// nastavi vzdy v nove spusteni aktivni prvni tab a focusneme prvni element v tabu pro navigaci
			SetCurrentTab(0,true);
		}
		else
		{
			// PANEL DISABLED
			DebugEnabledLabel.Text = "F1 for edit debug hud";
			OptionsPanel.Visible = false;

			// TRY CAST TO FPSCHARACTER INVENTORY a pokud mame aktualne otevreny inventory, preskocime zbytek kodu
			FPSCharacter_Inventory charInventory = CGameMaster.GM.GetGame().GetFPSCharacterOld() as FPSCharacter_Inventory;
			if(charInventory != null)
				if (charInventory.GetInventoryMenu().GetActive()) return;

            CGameMaster.GM.GetGame().GetFPSCharacterOld().SetInputEnable(true);
			CGameMaster.GM.GetGame().GetFPSCharacterOld().SetMouseVisible(false);
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	private void SetCurrentTab(int newTabID,bool focusOnFirstChild = false)
	{
        TabContainer debugHudTabs = GetNode<TabContainer>("OptionsPanel/TabContainer");
        if (debugHudTabs == null) return;

		if(newTabID < debugHudTabs.GetTabCount() && newTabID > -1)
		{
            debugHudTabs.GetCurrentTabControl().Hide();
            debugHudTabs.CurrentTab = newTabID;
            debugHudTabs.GetCurrentTabControl().Show();
        }

		// Focus on first element in tab (manualy)
		if(focusOnFirstChild)
		{
			if (debugHudTabs.GetCurrentTabControl().Name == "main")
				GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowFps_CheckBox").GrabFocus();
			else if (debugHudTabs.GetCurrentTabControl().Name == "level")
				debugHudTabs.GetCurrentTabControl().GetChild<Control>(0).GrabFocus();
			else if (debugHudTabs.GetCurrentTabControl().Name == "video")
				GetNode<OptionButton>("OptionsPanel/TabContainer/video/ScreenMode_HBoxContainer/ScreenMode_OptionButton").GrabFocus();
			else if (debugHudTabs.GetCurrentTabControl().Name == "audio")
				GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_HSlider").GrabFocus();
			else if (debugHudTabs.GetCurrentTabControl().Name == "inputs")
				GetNode<HSlider>("OptionsPanel/TabContainer/inputs/input_HBoxContainer/mouseSmooth_HSlider").GrabFocus();
        }
    }

	private int GetCurrentTabID()
	{
        return GetNode<TabContainer>("OptionsPanel/TabContainer").CurrentTab;
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

		CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO,
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
		Node3D worldlevel_occluderculling =
			CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene().GetLevelScene().GetLevelOcclusion();
		worldlevel_occluderculling.Visible = isPressed;

		CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO,
			"worldlevel_occluderculling set visible to: " + isPressed);
	}

	public void _on_show_performance_check_box_toggled(bool isPressed)
	{
		PerformancePanel.Visible = isPressed;
	}

	public void _on_quit_game_button_pressed()
	{
		CGameMaster.GM.QuitGame();
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

            VBoxContainer LevelButtonContainer = GetNode<VBoxContainer>("OptionsPanel/TabContainer/level");
            LevelButtonContainer.AddChild(b);
            b.SetLevelInfo(levelinfo);
        }

    }

	public void BuildSpawnButtons()
	{
		var allSpawnsInfo = UniversalFunctions.GetAllSpawnObjectsFromDir();
		foreach (var spawn in allSpawnsInfo)
		{
			spawn_object_button spawnButtonInstance = GD.Load<PackedScene>(
				"res://core_systems/debug_hud_system/spawn_object_button.tscn").Instantiate() as spawn_object_button;

			spawnButtonInstance.Text = spawn.name;
			spawnButtonInstance.SetSpawnObjectData(spawn.path, spawn.name);

			VBoxContainer spawnButtonContainer = GetNode<VBoxContainer>("OptionsPanel/TabContainer/spawn");
			spawnButtonContainer.AddChild(spawnButtonInstance);
		}
	}

    public void _on_h_slider_value_changed(float value)
	{
		Label a = GetNode<Label>("OptionsPanel/TabContainer/spawn/amountOfSPawnLabel/HSlider/Label2");
		a.Text = value.ToString();
	}

    public int GetNeedNumOfSpawn() 
	{
		HSlider a = GetNode<HSlider>("OptionsPanel/TabContainer/spawn/amountOfSPawnLabel/HSlider");
		return (int) a.Value;
	}

	public void CheckScreenModeSetting()
	{
		// pokud mame vybrany mod windowed, tak povolime moznost vybrat velikost okna, jinak ne
		if(CGameMaster.GM.GetSettings().GetActual_ScreenMode() == 0)
		{
			GetNode<OptionButton>("OptionsPanel/TabContainer/video/WindowSize_HBoxContainer/" +
				"WindowSize_OptionButton").Disabled = false;
			GetNode<OptionButton>("OptionsPanel/TabContainer/video/WindowSize_HBoxContainer/" +
				"WindowSize_OptionButton").Selected = CGameMaster.GM.GetSettings().GetActual_ScreenSizeID();
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
		CGameMaster.GM.GetSettings().Apply_ScreenMode(newID,true,false);
		CheckScreenModeSetting();	// volame pro logiku zapnuti/vypnuti moznosti vybirat velikost okna
	}

	// Signal pro zmenu rozliseni skrze option button
	public void _on_window_size_option_button_item_selected(int newID)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_ScreenSizeID(newID, true, false);
	}

	// Signal pro zmenu antialiasingu skrze option button
	public void _on_antialias_option_button_item_selected(int newID)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_AntialiasID(newID, true, false);
	}

    // Signal pro zmenu global ilumination skrze option button
    public void _on_gi_option_button_item_selected(int newID)
	{
		//only apply
		CGameMaster.GM.GetSettings().Apply_GlobalIlumination(newID, true, false);
    }

    public void _on_scale_3d_h_slider_value_changed(float newValue)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_Scale3D(newValue / 100.0f,true,false);

		Label scale3dLabel = GetNode<Label>("OptionsPanel/TabContainer/video/Scale3d_HBoxContainer/Scale3dvalue_Label");
		scale3dLabel.Text = (newValue / 100.0f).ToString();
	}

	public void _on_half_res_gi_check_box_toggled(bool newPressed)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_HalfResolutionGI(newPressed,true,false);
	}

	public void _on_ssao_check_box_toggled(bool newPressed)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_Ssao(newPressed,true,false);
	}

	public void _on_ssil_check_box_toggled(bool newPressed)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_Ssil(newPressed,true,false);
	}

	public void _on_unlock_max_fps_check_box_toggled(bool newPressed)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_UnlockMaxFps(newPressed,true,false);
	}

	public void _on_disable_vsync_check_box_toggled(bool newPressed)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_DisableVsync(newPressed, true, false);
	}

	public void _on_save_as_default_button_pressed()
	{
		// save all actual graphics settings
		CGameMaster.GM.GetSettings().SaveActual_AllGraphicsSettings();
	}

	public void ApplyAllVideoControls()
	{
		// screen mode
		OptionButton screenmode_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"ScreenMode_HBoxContainer/ScreenMode_OptionButton");
		screenmode_option.Selected = CGameMaster.GM.GetSettings().GetActual_ScreenMode();

		CheckScreenModeSetting();	// volame pro logiku zapnuti/vypnuti moznosti vybirat velikost okna

		// antialias 
		OptionButton antialias_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"Antialias_HBoxContainer/Antialias_OptionButton");
		antialias_option.Selected = CGameMaster.GM.GetSettings().GetActual_AntialiasID();

		OptionButton windowsize_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"WindowSize_HBoxContainer/WindowSize_OptionButton");
		windowsize_option.Selected = CGameMaster.GM.GetSettings().GetActual_ScreenSizeID();

		// gi
		OptionButton gi_option = GetNode<OptionButton>("OptionsPanel/TabContainer/video/" +
			"GI_HBoxContainer/GI_OptionButton");
		gi_option.Selected = CGameMaster.GM.GetSettings().GetActual_GlobalIlumination();

		// scale 3d
		HSlider scale3d_slider = GetNode<HSlider>("OptionsPanel/TabContainer/video/" +
			"Scale3d_HBoxContainer/Scale3d_HSlider");
		scale3d_slider.Value = CGameMaster.GM.GetSettings().GetActual_Scale3D() * 100.0f;

		Label scale3d_label = GetNode<Label>("OptionsPanel/TabContainer/video/Scale3d_HBoxContainer/Scale3dvalue_Label");
		scale3d_label.Text = CGameMaster.GM.GetSettings().GetActual_Scale3D().ToString();

		// half resolution gi
		CheckBox halfresgi_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/HalfResGI_CheckBox");
		halfresgi_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_HalfResolutionGI();

		// ssao
		CheckBox ssao_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/Ssao_CheckBox");
		ssao_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_Ssao();

		// ssil
		CheckBox ssil_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/Ssil_CheckBox");
		ssil_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_Ssil();

		CheckBox unlockmaxfps_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/UnlockMaxFps_CheckBox");
		unlockmaxfps_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_UnlockMaxFps();

		CheckBox vsync_checkbox = GetNode<CheckBox>("OptionsPanel/TabContainer/video/DisableVsync_CheckBox");
		vsync_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_DisableVsync();
	}

	public void _on_main_volume_h_slider_value_changed(float newValue)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_MainVolume(newValue, true, false);

		// update label
		Label mainVolume_label = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_Label");
		mainVolume_label.Text = newValue.ToString() + " db"; ;
	}

	public void _on_sfx_volume_h_slider_value_changed(float newValue)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_SfxVolume(newValue, true, false);

		// update label
		Label sfxVolume_label = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer2/sfxVolume_Label");
		sfxVolume_label.Text = newValue.ToString() + " db"; ;
	}

	public void _on_music_volume_h_slider_value_changed(float newValue)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_MusicVolume(newValue, true, false);

		// update label
		Label musicVolume_label = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer3/musicVolume_Label");
		musicVolume_label.Text = newValue.ToString() + " db"; ;
	}

	public void ApplyAllAudioControls()
	{
		// main volume
		HSlider mainVolumeSlider = GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_HSlider");
		mainVolumeSlider.Value = CGameMaster.GM.GetSettings().GetActual_MainVolume();

		Label mainVolumeLabel = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer/mainVolume_Label");
		mainVolumeLabel.Text = CGameMaster.GM.GetSettings().GetActual_MainVolume().ToString() + " db";

		// sfx volume
		HSlider sfxVolumeSlider = GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer2/sfxVolume_HSlider");
		sfxVolumeSlider.Value = CGameMaster.GM.GetSettings().GetActual_SfxVolume();

		Label sfxVolumeLabel = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer2/sfxVolume_Label");
		sfxVolumeLabel.Text = CGameMaster.GM.GetSettings().GetActual_SfxVolume().ToString() + " db"; ;

		// sfx volume
		HSlider musicVolumeSlider = GetNode<HSlider>("OptionsPanel/TabContainer/audio/audio_HBoxContainer3/musicVolume_HSlider");
		musicVolumeSlider.Value = CGameMaster.GM.GetSettings().GetActual_MusicVolume();

		Label musicVolumeLabel = GetNode<Label>("OptionsPanel/TabContainer/audio/audio_HBoxContainer3/musicVolume_Label");
		musicVolumeLabel.Text = CGameMaster.GM.GetSettings().GetActual_MusicVolume().ToString() + " db"; ;
	}

	public void _on_save_audio_as_default_button_pressed()
	{
		// save all actual graphics settings
		CGameMaster.GM.GetSettings().SaveActual_AllAudioSettings();
	}

	public void _on_mouse_smooth_h_slider_value_changed(float newValue)
	{
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookMouseSmooth(newValue,true,false);

        // update label
        Label label = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer/mouseSmooth_Label");
		label.Text = newValue.ToString();
    }

	public void _on_mouse_sensitivity_h_slider_value_changed(float newValue)
	{
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookMouseSensitivity(newValue,true,false);

        // update label
        Label label = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer2/mouseSensitivity_Label");
        label.Text = newValue.ToString();
    }

	public void _on_gamepad_smooth_h_slider_value_changed(float newValue)
	{
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookGamepadSmooth(newValue, true, false);

        // update label
        Label label = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer3/gamepadSmooth_Label");
        label.Text = newValue.ToString();
    }

	public void _on_gamepad_sensitivity_h_slider_value_changed(float newValue)
	{
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookGamepadSensitivity(newValue, true, false);

        // update label
        Label label = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer4/gamepadSensitivity_Label");
        label.Text = newValue.ToString();
    }

	public void _on_inverse_vertical_look_check_box_toggled(bool newValue)
	{
		// only apply
		CGameMaster.GM.GetSettings().Apply_InverseVerticalLook(newValue, true, false);
	}


    public void ApplyAllInputsControls()
	{
		HSlider msmooth = GetNode<HSlider>("OptionsPanel/TabContainer/inputs/input_HBoxContainer/mouseSmooth_HSlider");
		msmooth.Value = CGameMaster.GM.GetSettings().GetActual_LookMouseSmooth();
		Label msmooth_l = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer/mouseSmooth_Label");
		msmooth_l.Text = msmooth.Value.ToString();

        HSlider msens = GetNode<HSlider>("OptionsPanel/TabContainer/inputs/input_HBoxContainer2/mouseSensitivity_HSlider");
        msens.Value = CGameMaster.GM.GetSettings().GetActual_LookMouseSensitivity();
        Label msens_l = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer2/mouseSensitivity_Label");
        msens_l.Text = msens.Value.ToString();

        HSlider gsens = GetNode<HSlider>("OptionsPanel/TabContainer/inputs/input_HBoxContainer4/gamepadSensitivity_HSlider");
        gsens.Value = CGameMaster.GM.GetSettings().GetActual_LookGamepadSensitivity();
        Label gsens_l = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer4/gamepadSensitivity_Label");
        gsens_l.Text = gsens.Value.ToString();

        HSlider gsmooth = GetNode<HSlider>("OptionsPanel/TabContainer/inputs/input_HBoxContainer3/gamepadSmooth_HSlider");
        gsmooth.Value = CGameMaster.GM.GetSettings().GetActual_LookGamepadSmooth();
        Label gsmooth_l = GetNode<Label>("OptionsPanel/TabContainer/inputs/input_HBoxContainer3/gamepadSmooth_Label");
        gsmooth_l.Text = gsmooth.Value.ToString();

		CheckBox inverselook = 
			GetNode<CheckBox>("OptionsPanel/TabContainer/inputs/input_HBoxContainer5/inverseVerticalLook_CheckBox");
		inverselook.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_InverseVerticalLook();

    }

	public void _on_save_inputs_as_default_button_pressed()
    {
		CGameMaster.GM.GetSettings().SaveActual_AllInputsSettings();
	}


    public void ApplyAllMainControls()
	{
		// ziskame ulozena data
		global_settings_data data = CGameMaster.GM.GetSettings().GetData();

		SetEnable(data.ShowDebugHud);

		GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowFps_CheckBox").ButtonPressed = data.ShowFps;
		_on_show_performance_check_box_toggled(data.ShowPerformance);

		GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowPerformance_CheckBox").ButtonPressed = data.ShowPerformance;
/*
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
*/
		GetNode<CheckBox>("OptionsPanel/TabContainer/main/EnableWorldOcclusionCulling_CheckBox").ButtonPressed = 
			data.EnableWorldLevelOcclusionCull;
	}

	public void _on_save_main_as_default_button_pressed()
	{
		// ziskame ulozena data
		global_settings_data data = CGameMaster.GM.GetSettings().GetData();

		data.ShowDebugHud = isEnable;
		data.ShowFps = GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowFps_CheckBox").ButtonPressed;
		data.ShowPerformance = GetNode<CheckBox>("OptionsPanel/TabContainer/main/ShowPerformance_CheckBox").ButtonPressed;
		/*
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
		*/
		data.EnableWorldLevelOcclusionCull =
			GetNode<CheckBox>("OptionsPanel/TabContainer/main/EnableWorldOcclusionCulling_CheckBox").ButtonPressed;

		// ulozime data
		data.Save();
	}

}
