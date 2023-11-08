using Godot;
using System;

public partial class LevelScene : Node
{
    public Godot.Collections.Array<Node> GetAllRooms() { return GetNode<Node3D>("Rooms").GetChildren(); }
    public OccluderInstance3D GetLevelOcclusion() { return GetNode<OccluderInstance3D>("LevelOcclusion"); }
}
