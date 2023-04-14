using Godot;
using System;

public partial class InGameMenu : Control
{
	private bool active = false;

	public override void _Ready()
	{
		SetActive(false);
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
}
