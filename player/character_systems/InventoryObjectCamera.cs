using Godot;
using System;

public partial class InventoryObjectCamera : ObjectCamera
{
	Node3D inventoryPutItemPoint = null;
    public Node3D headWalkBobNode;
    public Node3D headBobBreathingNode;

    private HeadBobSystem headBobSystem = null;

    public override void _Ready()
	{
		base._Ready();

        headWalkBobNode = GetNode<Node3D>("%HeadWalkBob");
        headBobBreathingNode = GetNode<Node3D>("%HeadBobBreathing");
        inventoryPutItemPoint = GetNode<Node3D>("%InventoryPutItemPoint");

        headBobSystem = GetNode<HeadBobSystem>("%HeadBobSystem");
        headBobSystem.Init(this);
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        headBobSystem.Update((float)delta);
    }

    public Node3D GetInventoryItemPutPos()
	{
		return inventoryPutItemPoint;
	}

    public HeadBobSystem GetHeadBobSystem() { return headBobSystem; }
}
