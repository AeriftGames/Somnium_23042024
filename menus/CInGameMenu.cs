using Godot;
using Godot.Collections;
using System;

public partial class CInGameMenu : Control
{
	VBoxContainer InGameMenuButtonsContainer = null;

	private int focusButtonID = 0;
	private bool isOpen = false;

	public void PostInit()
	{
        InGameMenuButtonsContainer = GetNode<VBoxContainer>("Control/PanelContainer/MarginContainer/VBoxContainer");
        SetOpen(false);
	}

	public void ToggleOpen() { SetOpen(!isOpen); }

    public void SetOpen(bool newOpen)
	{
        // pokud mame otevreny debug panel - zavreme jej = main menu je nadrazene
        if (newOpen && CGameMaster.GM.GetGame().GetDebugPanel().GetIsOpen())
                CGameMaster.GM.GetGame().GetDebugPanel().OpenDebugTabs(false);

        // vnitrni zmena stavu
        isOpen = newOpen;

		// viditelnost InGameMenu
		Visible = isOpen;

		if(isOpen)
		{
            // for old fps character open
            if (CGameMaster.GM.GetGame().GetFPSCharacterOld() != null)
            {
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetInputEnable(false);
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetMouseVisible(true);
            }

            Input.MouseMode = Input.MouseModeEnum.Visible;

			if (CGameMaster.GM.GetGame().GetFPSCharacterBase() != null)
			{
                CGameMaster.GM.GetGame().GetFPSCharacterBase().SetCharacterInputState(
                    FpsCharacterBase.ECharacterInputState.InGameMenu);
            }
        }
		else
		{
            // for old fps character open
            if (CGameMaster.GM.GetGame().GetFPSCharacterOld() != null)
            {
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetInputEnable(true);
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetMouseVisible(false);

            }

            Input.MouseMode = Input.MouseModeEnum.Captured;

            if (CGameMaster.GM.GetGame().GetFPSCharacterBase() != null)
            {
				CGameMaster.GM.GetGame().GetFPSCharacterBase().SetCharacterInputState(
					FpsCharacterBase.ECharacterInputState.Normal);
            }
        }

        /*
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
        }*/
    }

	public bool GetIsOpen() { return isOpen; }

	// Button BackToGame - Deaktivuje toto InGameMenu a povoli opet inputs charactera
	public void _on_button_back_to_game_pressed()
	{
		SetOpen(false);
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
