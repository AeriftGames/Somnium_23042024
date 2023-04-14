using Godot;
using System;

public partial class InGameMenu : Control
{
	private bool active = false;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void SetActive(bool newActive)
	{
		active = newActive;
		Visible = active;

		// ziskame interact charactera
        FPSCharacter_Interaction interChar = (FPSCharacter_Interaction)GameMaster.GM.GetFPSCharacter();
		if (interChar == null) return;

        // jine pri zmene
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
			// povoli char_inputs zneviditelni mys
            interChar.SetInputEnable(true);
            //interChar.SetMouseVisible(false);
        }
	}

	public bool GetActive() { return active; }
}
