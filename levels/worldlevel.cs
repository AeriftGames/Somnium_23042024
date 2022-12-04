using Godot;
using System;

public partial class worldlevel : Node3D
{
	WorldEnvironment env = null;

	// Pokud je true = nastavi se na zacatku hry sdfgi na true. Pokud je false = ignorujeme a pouzivame z editoru
	[Export] bool InGameStartEnableSDFGI = true;

	public override void _Ready()
	{
		env = GetNode<WorldEnvironment>("WorldEnvironment");
		if(env != null)
		{
			if (InGameStartEnableSDFGI)
				env.Environment.SdfgiEnabled = true;
		}
	}
}
