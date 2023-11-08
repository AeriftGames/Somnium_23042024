using Godot;
using System;

[Tool]
public partial class SimpleStair : Node3D
{
    [Export] public int StairID = 0;

    public Vector3 GetStairEndGlobalPosition(){ return GetNode<Node3D>("StairEnd").GlobalPosition;}
    public Vector3 GetStairEndPosition() { return GetNode<Node3D>("StairEnd").Position; }
}
