using Godot;
using System;

public partial class interactive_object : Node3D
{
	private bool isPlayerInRange = false;

	public override void _Ready()
	{

	}

	public void _on_interactive_object_area_3d_body_entered(Node3D body)
	{
		if (body.IsClass("CharacterBody3D"))
		{
            GD.Print("Player is entered to area");
            isPlayerInRange = true;
        }
	}

	public void _on_interactive_object_area_3d_body_exited(Node3D body)
	{
        if (body.IsClass("CharacterBody3D"))
        {
            GD.Print("Player is exited area");
            isPlayerInRange = false;
        }
    }

	public bool GetIsActive()
	{
		return isPlayerInRange;
	}

	public void Use(FPSCharacter_Interaction player)
	{
        GetParent().Call("UseAction",player);
	}

	public string GetUseActionName()
	{
        Variant variant = GetParent().Call("GetUseActionName");
        string result = variant.AsString();
        variant.Dispose();
		return result;
    }

	public string GetInteractiveObjectName()
	{
        Variant variant = GetParent().Call("GetInteractiveObjectName");
        string result = variant.AsString();
        variant.Dispose();
        return result;
    }
}
