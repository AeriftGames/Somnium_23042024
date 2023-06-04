using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class light_wall : Node3D
{
	MeshInstance3D lightWallMesh = null;

	[Export] public bool _lightOn { get { return lightOn; } set {SetLightOn(value); } }
	bool lightOn = false;

	public override void _Ready()
	{
		lightWallMesh = GetNode<MeshInstance3D>("lightwall");
	}

	public override void _Process(double delta)
	{
	}

	public void SetLightOn(bool newLightOn)
	{
		GD.Print("halo");
        MeshInstance3D lightWallMesha = GetNode<MeshInstance3D>("lightwall");
        StandardMaterial3D lightGlassMaterial = lightWallMesha.GetActiveMaterial(1) as StandardMaterial3D;
		if (lightGlassMaterial == null) return;

		if(newLightOn)
		{
            lightGlassMaterial.EmissionEnergyMultiplier = 16;
			EnableParentOmniLights(true);
        }
		else 
		{
            lightGlassMaterial.EmissionEnergyMultiplier = 0;
            EnableParentOmniLights(false);
        }

		lightOn = newLightOn;
    }

	public bool GetLightOn() { return lightOn; }

	public void EnableParentOmniLights(bool newEnable)
	{
		foreach (var lightNode in GetChildren())
		{
			OmniLight3D omniLight = lightNode as OmniLight3D;
			if(omniLight != null)
			{
				omniLight.Visible = newEnable;
			}
		}
	}

	public void UseActionByButton()
	{
		SetLightOn(!GetLightOn());
	}

	public void UseAction(bool newAction)
	{ 
		SetLightOn(newAction);
	}
}
