using Godot;
using System;

public partial class InventoryObjectCamera : ObjectCamera
{
	Node3D inventoryPutItemPoint = null;
	[Export] float headBobbingWalkValue = 0.2f;
    [Export] float headBobbingSprintValue = 0.3f;
    [Export] float headBobbingDeltaToWalk = 1.0f;
    [Export] float headBobbingDeltaToStand = 3.0f;

	[Export] float headBobRotDegWalkValue = 2.0f;
    [Export] float headBobRotDegSprintValue = 4.0f;
    [Export] float headBobRotDeltaToWalk = 1.0f;
	[Export] float headBobRotDeltaToStand = 3.0f;
	float lerpTest = 0.0f;
	float headBobDelta = 1.0f;

	float headBobRot = 0.0f;
	float headBobRotDelta = 1.0f;

	Node3D headWalkBobNode;
	int lastFootState = 0;

	public override void _Ready()
	{
		base._Ready();

		headWalkBobNode = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean/ShakeNode/HeadWalkBob");

		inventoryPutItemPoint = GetCameraLookingPoint().GetNode<Node3D>("InventoryPutItemPoint");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

        headWalkBobNode.Position = headWalkBobNode.Position.Lerp(new Vector3(lerpTest, 0, 0), (float)delta * headBobDelta);
		headWalkBobNode.RotationDegrees = headWalkBobNode.RotationDegrees.Lerp(new Vector3(0, 0, headBobRot), (float)delta * headBobRotDelta);
    }

	public Node3D GetInventoryItemPutPos()
	{
		return inventoryPutItemPoint;
	}

    public override void UpdateWalkHeadBobbing(int actualStepState, float delta)
    {
        base.UpdateWalkHeadBobbing(actualStepState, delta);
		if (actualStepState == lastFootState) return;

		if (actualStepState == 0) 
		{
			// noha ve vzduchu
            lerpTest = 0.0f;
			headBobDelta = headBobbingDeltaToStand;
			headBobRot = 0.0f;
			headBobRotDelta = headBobRotDeltaToStand;
        }
		else if(actualStepState == 1)
		{
            // prava
			if(GetCharacterOwner().GetIsSprint())
			{
				//sprint
                lerpTest = headBobbingSprintValue;
                headBobRot = headBobRotDegSprintValue;
            }
			else
			{
				//walk
                lerpTest = headBobbingWalkValue;
                headBobRot = headBobRotDegWalkValue;
            }

			headBobDelta = headBobbingDeltaToWalk;
            headBobRot = headBobRotDegWalkValue;
            headBobRotDelta = headBobRotDeltaToWalk;
            GD.Print("prava");
        }
		else if(actualStepState == 2) 
		{
            // leva
            // prava
            if (GetCharacterOwner().GetIsSprint())
            {
                //sprint
                lerpTest = -headBobbingSprintValue;
                headBobRot = -headBobRotDegSprintValue;
            }
            else
            {
                //walk
                lerpTest = -headBobbingWalkValue;
                headBobRot = -headBobRotDegWalkValue;
            }

            headBobDelta = headBobbingDeltaToWalk;
            headBobRotDelta = headBobRotDeltaToWalk;
            GD.Print("leva");
        }

        lastFootState = actualStepState;
    }
}
