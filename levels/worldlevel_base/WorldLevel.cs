using Godot;
using System;
using System.Transactions;

public partial class WorldLevel : Node
{
	[Export] bool WithoutPlayer = false;

	private LevelScene levelScene;

    public WorldEnvironment GetWorldEnvironment() { return GetNode<WorldEnvironment>("WorldEnvironment"); }
    public VoxelGI GetVoxelGI() { return GetNode<VoxelGI>("VoxelGI"); }
    public LevelScene GetLevelScene() { return levelScene; }

	public override void _Ready()
	{
		levelScene = GetNode<LevelScene>("LevelScene");

		StartGameInit();
	}

	public void StartGameInit()
	{
		if(WithoutPlayer)
		{
			// Apply Settings
			GameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
			GameMaster.GM.EnableBlackScreen(false);
		}
	}

}
