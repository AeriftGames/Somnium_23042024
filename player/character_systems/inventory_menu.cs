using Godot;
using System;

public partial class inventory_menu : Control
{
	private bool _active = false;
    private bool active_nextFrame = false;
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
        SetActive(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (!GetActive()) return;

        // dovoli tento update az dalsi frame (kvuli inputu)
        if (!active_nextFrame) { active_nextFrame = true; return; }
        
        // close this inventory
        if(Input.IsActionJustPressed("toggleInventory"))
            SetActive(false);
	}

	public void SetActive(bool newActive) 
	{ 
		_active = newActive;
        Visible = newActive;

        // ziskame interact charactera
        FPSCharacter_Interaction interChar = (FPSCharacter_Interaction)GameMaster.GM.GetFPSCharacter();
        if (interChar == null) return;

        // ostatni akce pri zmene
        if (_active)
        {
            // vyresetuje lean a zoom hrace
            interChar.GetObjectCamera().SetActualLean(ObjectCamera.ELeanType.Center);
            interChar.SetCameraZoom(false);

            // zakaze char_inputs a zobrazi mys
            interChar.SetInputEnable(false);
            interChar.SetMouseVisible(true);

            //SetActiveFocusButtonID(0);
        }
        else
        {
            // povoli char_inputs + captured mouse (uvnitr funkce SetInputEnable)
            interChar.SetInputEnable(true);
            active_nextFrame = false;
        }
    }

    public bool GetActive() { return _active; }
}
