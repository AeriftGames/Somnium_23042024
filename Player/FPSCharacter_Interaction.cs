using Godot;
using System;

/*
 * *** FPSCharacter_Interaction(0.0) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and handle interaction with world
*/
public partial class FPSCharacter_Interaction : FPSCharacter_WalkingEffects
{
	BasicHud basicHud = null;

	[Export] public float LengthInteractRay = 5.0f;

	public override void _Ready()
	{
		base._Ready();

		basicHud = GetNode<BasicHud>("BasicHud");
        basicHud.SetUseVisible(false);
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

        basicHud.SetUseVisible(false);
        bool useNow = Input.IsActionJustPressed("UseAction");

		// otestujeme zdali existuje interactive_object, pokud ano otestujeme zdali je aktivni v range
		// pokud neco z toho neni pravda vyskocime z funkce
        interactive_object hit_interactive_object = DetectInteractiveObjectWithCameraRay();
		if (hit_interactive_object == null) return;
		if (hit_interactive_object.GetIsActive() == false) return;

		// pokud tedy mame pred sebou aktivni interactive_object, vypiseme jeho moznou akci v hudu
		basicHud.SetUseLabelText(hit_interactive_object.GetUseActionName());
        basicHud.SetUseVisible(true);

		// chceme interactive_object pouzit?
        if (useNow)
			hit_interactive_object.Use(this);
		
    }

    public interactive_object DetectInteractiveObjectWithCameraRay()
    {
		interactive_object result = null;
		if (GetFPSCharacterCamera() == null) return null;

        PhysicsDirectSpaceState3D directSpace = GetWorld3d().DirectSpaceState;
		if (directSpace == null) return null;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
		rayParam.From = GetFPSCharacterCamera().GlobalPosition;
		rayParam.To = GetFPSCharacterCamera().GlobalPosition - 
			GetFPSCharacterCamera().GlobalTransform.basis.z * LengthInteractRay;

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
		{
			Node HitCollider = (Node)rayResult["collider"];
			if (HitCollider == null) return null;

			if (HitCollider.GetParent() == null) return null;

			/*
			Type type = HitCollider.GetParent().GetType();
			if (type != typeof(interactive_object)) return null;
			*/

			if(HitCollider.GetParent().IsInGroup("interactive_object"))
			{
                result = (interactive_object)HitCollider.GetParent();
            }
		}

		return result;
    }

	public void DisableInputsAndCameraMoveLookTarget(Vector3 targetPos,Vector3 targetLook)
	{
		GD.Print("New Cam targetPos:" + targetPos);
		GD.Print("New Cam targetLook" + targetLook);
	}
}
