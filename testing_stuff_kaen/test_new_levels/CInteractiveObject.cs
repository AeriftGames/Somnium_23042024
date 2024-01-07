using Godot;
using System;

public partial class CInteractiveObject : Node3D
{
	[Export] public Node CallUseObject = null;
	[Export] private string ObjectName = "none";
	[Export] public string CallUseObjectFunction = "UseAction";
	[Export] public CBilboardObject BilboardObject = null;

	private bool isInRange = false;
	private bool isInLook = false;

    public void CallUseAction()
	{
		if (CallUseObject == null) { return; }

		else if (CallUseObject.HasMethod(CallUseObjectFunction) && isInRange && isInLook) 
		{ CallUseObject.Call(CallUseObjectFunction); }
	}

	public void SetIsInRange(bool newInRange) { isInRange = newInRange; }
	public bool GetIsInRange() { return isInRange; }
	public void SetIsInLook(bool newInLook) {  isInLook = newInLook; }
	public bool GetIsInLook() {  return isInLook; }
	public string GetObjectName() { return ObjectName; }

	public CBilboardObject GetBilboardObject() { return BilboardObject; }
}
