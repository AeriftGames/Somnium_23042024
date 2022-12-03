using Godot;
using System;

public partial class test_light : Node3D
{
	MeshInstance3D meshInstance3D;

	public override void _Ready()
	{
		meshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
	}

	public override void _Process(double delta)
	{
	}

    public void _on_wall_lever_test_lever_reach_end(bool newTop)
	{
		if(newTop)
		{
			//GREEN
			if(meshInstance3D != null)
				meshInstance3D.MaterialOverride.Set("albedo_color",Color.Color8(0,255,0,255));
		}
		else
		{
            //RED
            if (meshInstance3D != null)
                meshInstance3D.MaterialOverride.Set("albedo_color", Color.Color8(255, 0, 0, 255));
        }
	}
}
