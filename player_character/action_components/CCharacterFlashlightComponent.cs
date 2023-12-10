using Godot;
using System;
using System.Runtime.Remoting;

public partial class CCharacterFlashlightComponent : CBaseComponent
{
    public enum EFlashlightType { OnlyLight, HandObject}
    [Export] public EFlashlightType FLASHLIGHTTYPE = EFlashlightType.OnlyLight;

    Flashlight_Item FlashlightObject = null;

	private Vector3 workingLookAt = Vector3.Zero;
	private bool isEnable = false;

	public override void PostInit(FpsCharacterBase newCharacterBase)
	{
        base.PostInit(newCharacterBase);

        FlashlightObject = GetNode<Flashlight_Item>("Flashlight_Item");
        RemoveChild(FlashlightObject);
        ourCharacterBase.GetCharacterLookComponent().
            GetMainCamera().GetNode<Node3D>("RightHandPoint/RightHandObject").AddChild(FlashlightObject);

        if (FLASHLIGHTTYPE == EFlashlightType.OnlyLight)
        {
            FlashlightObject.ShowMesh(false);
            FlashlightObject.EnableFlashlight(false,true);
        }
        else if (FLASHLIGHTTYPE == EFlashlightType.HandObject)
        {
            FlashlightObject.ShowMesh(true);
            FlashlightObject.EnableFlashlight(false,true);
        }
    }

	public override void _Process(double delta)
	{
        // INPUT
        if (Input.IsActionJustPressed("ToggleFlashlight")) { FlashlightObject.UseAction(); }
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // UPDATE
        UpdateByCameraLook(delta);
    }

	private void UpdateByCameraLook(double delta)
	{
        // LERP LOOKINGPOINT FOR LOOKING
        workingLookAt = workingLookAt.Lerp(ourCharacterBase.GetCharacterLookComponent().GetMainCameraLookingPointPos(),
			(float)delta * 35.0f);

        ourCharacterBase.GetCharacterLookComponent().GetMainCamera().GetNode<Node3D>("RightHandPoint/RightHandObject").LookAt(workingLookAt);
    }
}
