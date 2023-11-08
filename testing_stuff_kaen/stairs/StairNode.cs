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
        }
    }

    Node3D AllStairsNode = null;

    public override void _Ready()
    {
        MyInit();
        GenerateStairs(NumStairs);
    }

    public void MyInit()
    {
        if (Engine.IsEditorHint())
        {
            Owner = GetTree().EditedSceneRoot;
            GetNode<Node3D>("AllStairsNodeT").Owner = GetTree().EditedSceneRoot;
        }
    }

    public void GenerateStairs(int newNum)
    {
        if (Engine.IsEditorHint())
        {
            if (GetNode<Node3D>("AllStairsNodeT") == null) return;

            // delete all
            if(GetNode<Node3D>("AllStairsNodeT").GetChildCount() > 0)
                foreach (var stair in GetNode<Node3D>("AllStairsNodeT").GetChildren())
                    stair.Free();
            
            GD.Print("Start Generate stairs from editor");
            PackedScene stair_prefab = GD.Load<PackedScene>("res://testing_stuff_kaen/stairs/stair_prefab_1.tscn");

            for (int i = 0; i < newNum; i++)
            {
                GD.Print("Generate " + i + " stair");
                SimpleStair newStair = stair_prefab.Instantiate<SimpleStair>();
                GetNode<Node3D>("AllStairsNodeT").AddChild(newStair);
                newStair.Owner = GetTree().EditedSceneRoot;
                newStair.Name = "Stair_"+i.ToString();

                if (i > 0)
                {
                    Vector3 endPos = (Vector3)GetNode<Node3D>("AllStairsNodeT").GetChild(i - 1).Call("GetStairEndGlobalPosition");
                    newStair.GlobalPosition = endPos;
                }
            }
        }
    }
}
