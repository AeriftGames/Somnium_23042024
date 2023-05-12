using Godot;
using System;

public partial class InventoryObjectCamera : ObjectCamera
{
	Node3D inventoryPutItemPoint = null;
	public override void _Ready()
	{
		base._Ready();

		inventoryPutItemPoint = GetCameraLookingPoint().GetNode<Node3D>("InventoryPutItemPoint");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public Node3D GetInventoryItemPutPos()
	{
		return inventoryPutItemPoint;
	}
}
