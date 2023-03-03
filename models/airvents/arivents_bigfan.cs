using Godot;
using System;

public partial class arivents_bigfan : Node3D
{
	MeshInstance3D fan1;
	MeshInstance3D fan2;

	[Export] float FanRotationSpeed = 1f;

	bool isRotate = false;

	public override void _Ready()
	{
		fan1 = GetNode<MeshInstance3D>("big_fan");
		fan2 = GetNode<MeshInstance3D>("big_fan001");
	}

	public override void _Process(double delta)
	{
		if (isRotate)
		{
			fan1.RotateX(FanRotationSpeed * (float)delta);
			fan2.RotateX(FanRotationSpeed * (float)delta);
		}
	}

	void _on_area_3d_body_entered(Node3D newBody)
	{
		GD.Print("ventilace zapnuta");
		isRotate = true;
	}

	void _on_area_3d_body_exited(Node3D newBody)
	{
		GD.Print("ventilace vypnuta");
		isRotate = false;
	}
}
