using Godot;
using System;

public partial class test_bilboard : Node3D
{
	Node3D NodeLeftUp;
	Node3D NodeRightDown;
	private bool LookAtCamera = false;
	[Export] public bool CanLookAtCamera = true;

	public override void _Ready()
	{
		NodeLeftUp = GetNode<Node3D>("NodeLeftUp");
        NodeRightDown = GetNode<Node3D>("NodeRightDown");

		ActivateLookAtCamera(false);
    }

	public override void _Process(double delta)
	{
		if (CanLookAtCamera && LookAtCamera)
		{ 
			LookAt(CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterLookComponent().
			GetMainCamera().GlobalPosition);
			GD.Print("LookAtCameraActivate");
		}
	}

	public Vector3 GetLeftUpPosition() { return NodeLeftUp.GlobalPosition; }
    public Vector3 GetRightDownPosition() { return NodeRightDown.GlobalPosition; }

	public void ActivateLookAtCamera( bool newActivate ){ LookAtCamera = newActivate; }
}
