using Godot;
using System;

public partial class CInteractiveObject : Node3D
{
    public enum EInteractiveLevel { Disable, OnlyUse, OnlyPhysic, UseAndPhysic }
    public enum EInteractivePhysicType { GrabItem, GrabJoint, GrabAction }
    public enum EUseInteractVisibleBy { Text, HandClick, HandClickAndText }

    [Export] public Node CallUseObject = null;
	[Export] private string ObjectName = "none";
    [Export] private string UseActionName = "none";
    [Export] public EInteractiveLevel InteractiveLevel = EInteractiveLevel.OnlyUse;
    [Export] public EInteractivePhysicType InteractivePhysicType = EInteractivePhysicType.GrabItem;
    [Export] public EUseInteractVisibleBy InteractVisibleBy = EUseInteractVisibleBy.Text;
    [Export] public Node3D InteractCenterNode = null;
    [Export] public bool UseOffsetHitInteract = false;

    private bool isInRange = false;
	private bool isInLook = false;

    public void CallUseAction() { CallActionFunction("UseAction"); }
	public void SetIsInRange(bool newInRange) { isInRange = newInRange; }
	public bool GetIsInRange() { return isInRange; }
	public void SetIsInLook(bool newInLook) {  isInLook = newInLook; }
	public bool GetIsInLook() {  return isInLook; }
	public string GetObjectName() { return ObjectName; }
    public string GetUseActionName() { return UseActionName; }
	public void CallActionFunction(string newActionFunction)
	{
        if (CallUseObject == null) { return; }

        if (isInRange && isInLook)
            UniversalFunctions.TryCall(CallUseObject, newActionFunction);
    }
    public Vector3 GetInteractCenterGlobalPosition() 
    { 
        if(InteractCenterNode == null) 
        {
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(
                this, CMasterLog.ELogMsgType.ERROR, "neexistuje interact center node");
        }

        return InteractCenterNode.GlobalPosition; 
    }
}
