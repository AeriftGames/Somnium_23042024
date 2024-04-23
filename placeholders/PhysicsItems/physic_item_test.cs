using Godot;
using Godot.Collections;
using System;

public partial class physic_item_test : RigidBody3D
{
	[Export] string UseActionText = "no action";
	[Export] string ObjectName = "box";

	[Export] public Array<AudioStream> PhysicEffectsAudioStream;

	interactive_object interactiveObject;

	[Export] public bool DisableCollisionToPlayer = true;

	[Export] public Array<NodePath> callUseNodes = null;

	//
	bool isGrab = false;

	public override void _Ready()
	{
		interactiveObject = GetNode<interactive_object>("interactive_object");

		SetDisableCollisionToPlayer(DisableCollisionToPlayer);
	}

	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{

	}

	public void UseAction(FPSCharacter_Interaction character)
	{
		CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO, "USE");

		foreach (NodePath callNode in callUseNodes)
			GetNode(callNode).Call("Use");
	}

	public void ApplyGrab(bool newGrab,FPSCharacter_Interaction character)
	{
		isGrab = newGrab;
	}

	public void GrabUpdate(double delta)
	{
	}

	public void SetDisableCollisionToPlayer(bool newDisableCollisionToPlayer)
	{
		DisableCollisionToPlayer = newDisableCollisionToPlayer;

		if (newDisableCollisionToPlayer)
		{
			// layer pro physica objekty
			SetCollisionLayerValue(1, false);
			SetCollisionLayerValue(2, false);
			SetCollisionLayerValue(3, false);
			SetCollisionLayerValue(4, true);
			// maska se statickym svetem a s physical objekty
			SetCollisionMaskValue(1, true);
			SetCollisionMaskValue(2, false);
			SetCollisionMaskValue(3, false);
			SetCollisionMaskValue(4, true);
		}
		else
		{
			// layer pro physica objekty
			SetCollisionLayerValue(1, false);
			SetCollisionLayerValue(2, false);
			SetCollisionLayerValue(3, false);
			SetCollisionLayerValue(4, true);
			// maska se statickym svetem a s physical objekty
			SetCollisionMaskValue(1, true);
			SetCollisionMaskValue(2, true);
			SetCollisionMaskValue(3, false);
			SetCollisionMaskValue(4, true);
		}
	}


	public void message_update()
	{
		string msg = interactiveObject.msgObject.GetMessage();
		switch (msg)
		{
			case "msg_get_use_action_text":
				{
					interactiveObject.msgObject.SetStringData(UseActionText);
					break;
				}
			case "msg_get_interactive_object_name":
				{
					interactiveObject.msgObject.SetStringData(ObjectName);
					break;
				}
			case "msg_use_action":
				{
					UseAction((FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData());
					break;
				}
			case "msg_apply_grab":
				{
					ApplyGrab(interactiveObject.msgObject.GetBoolData(),
						(FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData());
					break;
				}
			default: break;
		}
	}
}
