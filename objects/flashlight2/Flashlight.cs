using Godot;
using System;

public partial class Flashlight : Node3D
{
	MeshInstance3D meshLightInside = null;
	SpotLight3D spotlight = null;

	private bool isEnable = false;

	public override void _Ready()
	{
		meshLightInside = GetNode<MeshInstance3D>("Cylinder/light_inside");
		spotlight = GetNode<SpotLight3D>("Cylinder/SpotLight3D");

		// Nastavi na zacatku baterku na vypnutou
		SetEnable(false);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("testFlashlight"))
			ToggleEnable();
	}

	public void SetEnable( bool newEnable )
	{ 
		isEnable = newEnable;

		if(isEnable)
		{
			spotlight.Visible = true;
			meshLightInside.Visible = true;
		}
		else
		{
			spotlight.Visible = false;
			meshLightInside.Visible = false;
		}
	}

	public bool GetIsEnable()
	{
		return isEnable;
	}

	public bool ToggleEnable()
	{ 
		SetEnable(!isEnable);
		return isEnable; 
	}
}
