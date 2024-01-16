using Godot;
using System;

public partial class CInteractiveObject : Node3D
{
	[Export] public Node CallUseObject = null;
	[Export] private string ObjectName = "none";
	[Export] public CBilboardObject BilboardObject = null;
	[Export] public Godot.Collections.Array<CBaseAction> ActionResources;

	private bool isInRange = false;
	private bool isInLook = false;

    public void CallUseAction()
	{
		CallActionFunction("UseAction");
	}

	public void SetIsInRange(bool newInRange) { isInRange = newInRange; }
	public bool GetIsInRange() { return isInRange; }
	public void SetIsInLook(bool newInLook) {  isInLook = newInLook; }
	public bool GetIsInLook() {  return isInLook; }
	public string GetObjectName() { return ObjectName; }

	public CBilboardObject GetBilboardObject() { return BilboardObject; }

	public Godot.Collections.Array<CBaseAction> GetAllActionResources(){ return ActionResources; }
	public void CallActionFunction(string newActionFunction)
	{
        if (CallUseObject == null) { return; }

        if (CallUseObject.HasMethod(newActionFunction) && isInRange && isInLook)
		{ CallUseObject.Call(newActionFunction); }
    }
}
