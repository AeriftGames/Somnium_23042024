using Godot;
using System;

public partial class interactive_item_test : Node3D
{
	[Export] public string ObjectName = "item";
	[Export] public string UseActionName = "use";

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void UseAction(FPSCharacter_Interaction player)
	{
		GD.Print("Use Action by: " + player.Name);
		GD.Print("Destroying this item");
		QueueFree();
	}

    public string GetInteractiveObjectName() 
	{ 
		return Name;
	}

    public string GetUseActionName()
	{
		return UseActionName;
	}
}
