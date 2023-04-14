using Godot;
using Godot.Collections;
using System;

public partial class InGameMenu : Control
{
	VBoxContainer InGameMenuButtonsContainer = null;

	private int focusButtonID = 0;
	private bool active = false;

	public override void _Ready()
	{
		InGameMenuButtonsContainer = GetNode<VBoxContainer>("Control/VBoxContainer");

        SetActive(false);
	}

    public override void _Process(double delta)
    {
    }

    public void SetActive(bool newActive)
	{
		// vnitrni zmena stavu
		active = newActive;

		// viditelnost InGameMenu
		Visible = active;

		// ziskame interact charactera
        FPSCharacter_Interaction interChar = (FPSCharacter_Interaction)GameMaster.GM.GetFPSCharacter();
		if (interChar == null) return;

        // ostatni akce pri zmene
        if (active)
		{
			// vyresetuje lean a zoom hrace
			interChar.GetObjectCamera().SetActualLean(ObjectCamera.ELeanType.Center);
			interChar.SetCameraZoom(false);

			// zakaze char_inputs a zobrazi mys
            interChar.SetInputEnable(false);
            interChar.SetMouseVisible(true);

			SetActiveFocusButtonID(0);
        }
		else
		{
			// povoli char_inputs + captured mouse (uvnitr funkce SetInputEnable)
            interChar.SetInputEnable(true);
        }
	}

	public bool GetActive() { return active; }

	// Button BackToGame - Deaktivuje toto InGameMenu a povoli opet inputs charactera
	public void _on_button_back_to_game_pressed()
	{
		SetActive(false);
	}

    public void _on_button_quit_game_pressed()
	{
		GameMaster.GM.QuitGame();
	}

	public void SetActiveFocusButtonID(int newButtonID)
	{
		focusButtonID = newButtonID;

		foreach (var item in InGameMenuButtonsContainer.GetChildren())
		{
			BaseFocusedMenuButton a = (BaseFocusedMenuButton)item;
			if (a != null)
			{
				if(a.ButtonFocusID == newButtonID)
				{
                    GD.Print("FOCUS");
                    a.GrabFocus();
				}
			}
		}
	}

	public int GetFocusButtonID()
	{
		return focusButtonID;
	}
}
