using Godot;
using System;

public partial class interactive_object : Node3D
{
	// komunikacni objekt
	public MessageObject msgObject;

	private bool isPlayerInRange = false;

	public override void _Ready()
	{
		msgObject = new MessageObject(this,GetParent());
	}

	public void _on_interactive_object_area_3d_body_entered(Node3D body)
	{
		if (body.IsClass("CharacterBody3D"))
		{
			GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "Player is entered to area");
			isPlayerInRange = true;
		}
	}

	public void _on_interactive_object_area_3d_body_exited(Node3D body)
	{
		if (body.IsClass("CharacterBody3D"))
		{
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "Player is exited area");
			isPlayerInRange = false;
		}
	}

	public bool GetIsActive()
	{
		return isPlayerInRange;
	}

	public void Use(FPSCharacter_Interaction player)
	{
		// nastavime node data jako playera a posleme zpravu o use_action dal
		msgObject.SetNodeData(player);
		msgObject.SendMessageToGDNow("msg_use_action");
	}

	public string GetUseActionName()
	{
		string result = msgObject.LoadStringDataFromGDNow("msg_get_use_action_text");
		return result;
	}

	public string GetInteractiveObjectName()
	{
		string result = msgObject.LoadStringDataFromGDNow("msg_get_interactive_object_name");
		return result;
	}

	public void message_update()
	{
		string msg = msgObject.GetMessage();
		switch (msg)
		{
			default: break;
		}
	}
}
