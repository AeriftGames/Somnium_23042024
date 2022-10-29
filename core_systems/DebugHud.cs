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

		ShowFpsCheckBox = GetNode<CheckBox>("OptionsPanel/VBoxContainer/ShowFps_CheckBox");

		InitAllCustomLabels();

        OptionsPanel.Visible = false;
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

	private void UpdateTimer()
	{
        FPSLabel.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
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
}
