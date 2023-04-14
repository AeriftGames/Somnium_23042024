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
			
		}
		else
		{

        }
	}

	public bool GetActive() { return active; }
}
