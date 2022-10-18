using Godot;
using System;

/*
 * *** FPSCharacter_Interaction(0.0) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and handle interaction with world
*/
public partial class FPSCharacter_Interaction : FPSCharacter_WalkingEffects
{
	[Export] public float LengthInteractRay = 5.0f;

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if(Input.IsActionJustPressed("UseAction"))
		{
			//
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		interactive_object hit_interactive_object = DetectInteractiveObjectWithCameraRay();
		if (hit_interactive_object != null)
		{
			if(hit_interactive_object.GetIsActive())
			{
				// input USE
			}
		}

    }

    public interactive_object DetectInteractiveObjectWithCameraRay()
    {
		interactive_object result = null;
		if (GetFPSCharacterCamera() == null) return null;

        PhysicsDirectSpaceState3D directSpace = GetWorld3d().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
		rayParam.From = GetFPSCharacterCamera().GlobalPosition;
		rayParam.To = GetFPSCharacterCamera().GlobalPosition - 
			GetFPSCharacterCamera().GlobalTransform.basis.z * LengthInteractRay;

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
		{
			Node3D HitCollider = (Node3D)rayResult["collider"];
            if(HitCollider.GetParent() != null)
			{
				Type type = HitCollider.GetParent().GetType();
				if(type == typeof(interactive_object))
				{
					result = (interactive_object)HitCollider.GetParent();
				}
            }

		}

		return result;
    }
}
