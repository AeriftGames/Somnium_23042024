using Godot;
using System;

public partial class InventoryObjectCamera : ObjectCamera
{
	Node3D inventoryPutItemPoint = null;
    public Node3D headWalkBobNode;
    public Node3D headBobBreathingNode;

    private HeadBobSystem headBobSystem = null;
    private HeadDangerShakeSystem headDangerShakeSystem = null;
    private HeadStairsBobComponent headStairsBobComponent = null;

    public override void _Ready()
	{
		base._Ready();

        headWalkBobNode = GetNode<Node3D>("%HeadWalkBob");
        headBobBreathingNode = GetNode<Node3D>("%HeadBobBreathing");
        inventoryPutItemPoint = GetNode<Node3D>("%InventoryPutItemPoint");

        headBobSystem = GetNode<HeadBobSystem>("%HeadBobSystem");
        headDangerShakeSystem = GetNode<HeadDangerShakeSystem>("%HeadDangerShakeSystem");
        headBobSystem.StartInit(this);

        headStairsBobComponent = GetNode<HeadStairsBobComponent>("%HeadStairsBobComponent");
        headStairsBobComponent.StartInit(this);
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        headStairsBobComponent.Update((float)delta);
        headBobSystem.Update((float)delta);
    }

    public Node3D GetInventoryItemPutPos()
	{
		return inventoryPutItemPoint;
	}

    public HeadBobSystem GetHeadBobSystem() { return headBobSystem; }
    public HeadDangerShakeSystem GetHeadDangerShakeSystem() { return headDangerShakeSystem; }

    public HeadStairsBobComponent GetHeadStairsBobComponent() {  return headStairsBobComponent; }
}
