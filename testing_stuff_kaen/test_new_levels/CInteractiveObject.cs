using Godot;
using System;

public partial class CInteractiveObject : Node3D
{
	[Export] public Node CallUseObject = null;
	[Export] public MeshInstance3D OutlineMesh = null;
	[Export] private string ObjectName = "none";
	[Export] public string CallUseObjectFunction = "UseAction";

	private bool isInRange = false;
	private bool isInLook = false;

    public override void _Ready()
    {
        base._Ready();

		// unique outline material
		if (OutlineMesh != null)
            OutlineMesh.MaterialOverlay = OutlineMesh.MaterialOverlay.Duplicate() as Material;
    }

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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

		//SetIsInLook(false);
    }

	public void SetOutlineEffect(bool newOutlineEnable)
	{
		if(OutlineMesh != null) 
		{
			if(OutlineMesh.MaterialOverlay != null)
			{ OutlineMesh.MaterialOverlay.Set("shader_parameter/enable", newOutlineEnable); }
		}
	}
}
