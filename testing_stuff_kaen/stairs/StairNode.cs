using Godot;
using System;

[Tool]
public partial class StairNode : Node
{
    private int _NumStairs = 0;

    [Export]
    public int NumStairs
    {
        get => _NumStairs;
        set
        {
            _NumStairs = value;
            GenerateStairs(value);
        }
    }

    Node3D AllStairsNode;

    public override void _Ready()
    {
    }

    public void GenerateStairs(int newNum)
    {
        if (Engine.IsEditorHint())
        {
            GetNode<Node3D>("AllStairsNode").Owner = GetTree().EditedSceneRoot;

            // delete all
            if(GetNode<Node3D>("AllStairsNode").GetChildCount() > 0)
                foreach (var stair in GetNode<Node3D>("AllStairsNode").GetChildren())
                    stair.Free();

            GD.Print("Start Generate stairs from editor");
            PackedScene stair_prefab = GD.Load<PackedScene>("res://testing_stuff_kaen/stairs/stair_prefab_1.tscn");

            for (int i = 0; i < newNum; i++)
            {
                GD.Print("Generate " + i + " stair");
                SimpleStair newStair = stair_prefab.Instantiate<SimpleStair>();
                GetNode<Node3D>("AllStairsNode").AddChild(newStair);
                newStair.Owner = GetTree().EditedSceneRoot;
                newStair.Name = "Stair_"+i.ToString();

                if (i > 0)
                {
                    Vector3 endPos = (Vector3)GetNode<Node3D>("AllStairsNode").GetChild(i - 1).Call("GetStairEndGlobalPosition");
                    newStair.GlobalPosition = endPos;
                }
            }
        }
    }
}
