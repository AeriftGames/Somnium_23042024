using Godot;
using System;

public partial class interactive_item_look_and_interact_test : Node3D
{
	[Export] public string ObjectName = "look and interact_test_object";
    [Export] public string UseActionName = "look and interact";

    Node3D TargetCamPos;
    Node3D TargetCamLook;
    bool isNowInteract = false;

    FPSCharacter_Interaction interactPlayer = null;

    public override void _Ready()
	{
        TargetCamPos = GetNode<Node3D>("TargetCamPos");
        TargetCamLook = GetNode<Node3D>("TargetCamLook");

    }

	public override void _Process(double delta)
	{
        if (!isNowInteract) return;

        if (Input.IsActionJustPressed("Jump"))
        {
            interactPlayer.Call("EnableInputsAndCameraToNormal");
            interactPlayer = null;
            isNowInteract = false;
        }
	}

    public void UseAction(FPSCharacter_Interaction player)
    {
        if (player == null) return;
        interactPlayer = player;
        
        if(!isNowInteract)
        {
            player.Call("DisableInputsAndCameraMoveLookTarget",
                TargetCamPos.GlobalPosition, TargetCamLook.GlobalPosition);
            isNowInteract = true;
        }
    }

    public string GetInteractiveObjectName()
    {
        return ObjectName;
    }

    public string GetUseActionName()
    {
        return UseActionName;
    }
}
