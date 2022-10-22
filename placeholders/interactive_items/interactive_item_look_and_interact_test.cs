using Godot;
using System;

public partial class interactive_item_look_and_interact_test : Node3D
{
	[Export] public string ObjectName = "look and interact_test_object";
	[Export] public string UseActionText = "look and interact";

	// objekt s kterym komunikujeme
	interactive_object inter_object;

	Node3D TargetCamPos;
	Node3D TargetCamLook;
	bool isNowInteract = false;

	FPSCharacter_Interaction interactPlayer = null;

	public override void _Ready()
	{
		inter_object = GetNode<interactive_object>("interactive_object");

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

	public void message_update()
	{
		string msg = inter_object.msgObject.GetMessage();
		switch(msg)
		{
			case "msg_use_action":
				{
					UseAction((FPSCharacter_Interaction)inter_object.msgObject.GetNodeData());
					break;
				}
			case "msg_get_use_action_text":
				{
					inter_object.msgObject.SetStringData(UseActionText);
					break;
				}
			case "msg_get_interactive_object_name":
				{
					inter_object.msgObject.SetStringData(ObjectName);
					break;
				}
			default: break;
		}
	}
}
