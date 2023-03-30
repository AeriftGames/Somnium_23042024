using Godot;
using System;
using System.Transactions;

public partial class worldlevel : Node3D
{
	WorldEnvironment env = null;

	// Pokud je true = nastavi se na zacatku hry sdfgi na true. Pokud je false = ignorujeme a pouzivame z editoru
	[Export] bool InGameStartEnableSDFGI = false;

	Node VoxelGINode = null;

	public override void _Ready()
	{
		env = GetNode<WorldEnvironment>("WorldEnvironment");
		if(env != null)
		{
			if (InGameStartEnableSDFGI)
			{
				env.Environment.SdfgiEnabled = true;
				GameMaster.GM.LevelLoader.SDFGI = true;
			}
		}
	}
}
