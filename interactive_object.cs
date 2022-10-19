using Godot;
using System;

public partial class interactive_object : Node3D
{
	public bool isPlayerInRange = false;
	Node3D parent = null;

	public override void _Ready()
	{
		parent = (Node3D)GetParent();
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
		parent.Call("UseAction",player);
	}

	public string GetUseActionName()
	{
        Variant a = GetParent().Call("GetUseActionName");
        string b = a.AsString();
        a.Dispose();
		return b;
    }

	public string GetInteractiveObjectName()
	{
		string text = "";
        return text;
    }
}
