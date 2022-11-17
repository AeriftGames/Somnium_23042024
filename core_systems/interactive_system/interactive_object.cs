using Godot;
using System;

public partial class interactive_object : Node3D
{
	// komunikacni objekt
	public MessageObject msgObject;

	public enum EInteractiveLevel { Disable, OnlyUse, OnlyPhysic, UseAndPhysic}
	public enum EInteractivePhysicType { GrabItem, GrabJoint}

	[Export] public EInteractiveLevel InteractiveLevel = EInteractiveLevel.OnlyUse;
	[Export] public EInteractivePhysicType InteractivePhysicType = EInteractivePhysicType.GrabItem;

    private bool isPlayerInRange = false;

	public override void _Ready()
	{
		msgObject = new MessageObject(this,GetParent());
	}

	public void _on_interactive_object_area_3d_body_entered(Node3D body)
	{
		if (InteractiveLevel == EInteractiveLevel.Disable) return;

        if (body.IsClass("CharacterBody3D"))
		{
			GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "Player is entered to area");
			isPlayerInRange = true;
		}
	}

	public void _on_interactive_object_area_3d_body_exited(Node3D body)
	{
		if (InteractiveLevel == EInteractiveLevel.Disable) return;

        if (body.IsClass("CharacterBody3D"))
		{
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "Player is exited area");
			isPlayerInRange = false;
		}
	}

	public bool GetIsPlayerInRange()
	{
		return isPlayerInRange;
	}

	public void Use(FPSCharacter_Interaction player)
	{
		if (InteractiveLevel == EInteractiveLevel.Disable) return;
        if (InteractiveLevel == EInteractiveLevel.OnlyUse || InteractiveLevel == EInteractiveLevel.UseAndPhysic)
		{
            // nastavime node data jako playera a posleme zpravu o use_action dal
            msgObject.SetNodeData(player);
            msgObject.SendMessageToGDNow("msg_use_action");
        }
	}

	public string GetUseActionName()
	{
		if (InteractiveLevel == EInteractiveLevel.Disable) return "";

        string result = msgObject.LoadStringDataFromGDNow("msg_get_use_action_text");
		return result;
	}

	public string GetInteractiveObjectName()
	{
        if (InteractiveLevel == EInteractiveLevel.Disable) return "";
		if (!isPlayerInRange) return "";

        string result = msgObject.LoadStringDataFromGDNow("msg_get_interactive_object_name");
		return result;
	}

    public void ApplyGrab(bool newGrab, FPSCharacter_Interaction character)
    {
        msgObject.SetBoolData(newGrab);
        msgObject.SetNodeData(character);
        msgObject.SendMessageToGDNow("msg_apply_grab");
    }

    public virtual void message_update()
	{
		string msg = msgObject.GetMessage();
		switch (msg)
		{
			default: break;
		}
	}
}
