using Godot;
using System;

public partial class interactive_item_look_and_interact_test : Node3D
{
	[Export] public string ObjectName = "look and interact_test_object";
    [Export] public string UseActionName = "look and interact";

    Node3D TargetCamPos;
    Node3D TargetCamLook;

    public override void _Ready()
	{
        TargetCamPos = GetNode<Node3D>("TargetCamPos");
        TargetCamLook = GetNode<Node3D>("TargetCamLook");

    }

	public override void _Process(double delta)
	{
	}

    public void UseAction(FPSCharacter_Interaction player)
    {
        GD.Print("Object: " + ObjectName + " UseAction!");

        if (player == null) return;

        player.Call("DisableInputsAndCameraMoveLookTarget",TargetCamPos.GlobalPosition, TargetCamLook.GlobalPosition);
    }

    public string GetInteractiveObjectName()
    {
        return ObjectName;
    }

    public string GetUseActionName()
    {
        return UseActionName;
    }
}
