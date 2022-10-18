using Godot;
using System;

public partial class interactive_object : Node3D
{
	public bool isPlayerInRange = false;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void _on_interactive_object_area_3d_body_entered(Node3D body)
	{
		if (body.IsClass("CharacterBody3D"))
		{
            //GD.Print("Player is entered to area");
            isPlayerInRange = true;
        }
	}

	public void _on_interactive_object_area_3d_body_exited(Node3D body)
	{
        if (body.IsClass("CharacterBody3D"))
        {
            //GD.Print("Player is exited area");
            isPlayerInRange = false;
        }
    }

	public bool GetIsActive()
	{
		return isPlayerInRange;
	}

	public void Use(FPSCharacter_Interaction player)
	{
		if (GetParent() == null) return;
		GetParent().Call("UseAction",player);
	}

	public string GetUseActionName()
	{
		return "test";
	}

	public string GetInteractiveObjectName()
	{
		return "InteractiveName";
	}
}
