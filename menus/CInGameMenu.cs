using Godot;
using Godot.Collections;
using System;

public partial class CInGameMenu : Control
{
	VBoxContainer InGameMenuButtonsContainer = null;

	private int focusButtonID = 0;
	private bool active = false;

	public void PostInit()
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

		// pokusime se ziskat interact charactera
        FPSCharacter_Interaction interChar = CGameMaster.GM.GetGame().GetFPSCharacter() as FPSCharacter_Interaction;
		if (interChar == null) return;

        // ostatni akce pri zmene
        if (active)
		{
			// pokusime se castit na inventory player (pokud je hrac inventory - ma otevreny inventar? pokud ano, zavreme ho)
			FPSCharacter_Inventory invChar = interChar as FPSCharacter_Inventory;
            if (invChar != null)
				if (invChar.GetInventoryMenu().GetActive())
					invChar.GetInventoryMenu().SetActive(false);
			
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
			// jen pokud je hrac typu: (inventory a vys) a pokud je nazivu
			FPSCharacter_Inventory invChar = interChar as FPSCharacter_Inventory;
			if (invChar != null)
				if(invChar.GetCharacterHealthComponent().GetAlive())
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
		CGameMaster.GM.QuitGame();
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
                    //GD.Print("FOCUS");
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
