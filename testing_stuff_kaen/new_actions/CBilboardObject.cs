using Godot;
using System;

public partial class CBilboardObject : Node3D
{
    [Export] public bool CanLookAtCamera = true;

    private Node3D NodeLeftUp;
    private Node3D NodeRightDown;
    private Node3D NodeLeftDown;
    private Node3D NodeRightUp;

    private bool LookAtCamera = false;

    public override void _Ready()
    {
        NodeLeftUp = GetNode<Node3D>("NodeLeftUp");
        NodeRightDown = GetNode<Node3D>("NodeRightDown");
        NodeLeftDown = GetNode<Node3D>("NodeLeftDown");
        NodeRightUp = GetNode<Node3D>("NodeRightUp");

        ActivateLookAtCamera(false);
    }

    public override void _Process(double delta)
    {
        if (CanLookAtCamera && LookAtCamera)
        {
            LookAt(CGameMaster.GM.GetGame().GetFPSCharacterBase().GetCharacterLookComponent().
            GetMainCamera().GlobalPosition);
            //GD.Print("LookAtCameraActivate");
        }
    }
    public Vector3 GetLeftUpPosition() { return NodeLeftUp.GlobalPosition; }
    public Vector3 GetRightDownPosition() { return NodeRightDown.GlobalPosition; }
    public Vector3 GetLeftDownPosition() { return NodeLeftDown.GlobalPosition; }
    public Vector3 GetRightUpPosition() { return NodeRightUp.GlobalPosition; }

    public void ActivateLookAtCamera(bool newActivate) { LookAtCamera = newActivate; }
}