using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;

public partial class WorldLevel : Node
{
	public enum ELevelType { GameLevel,BenchmarkLevel,Menu}

	[Export] bool WithoutPlayer = false;
	[Export] public ELevelType LevelType = ELevelType.GameLevel;

	private LevelScene levelScene;

	public WorldEnvironment GetWorldEnvironment() { return GetNode<WorldEnvironment>("WorldEnvironment"); }
	public VoxelGI GetVoxelGI() { return GetNode<VoxelGI>("VoxelGI"); }
	public LevelScene GetLevelScene() { return levelScene; }
	public ELevelType GetLevelType() { return LevelType; }
	public LevelDataSettings GetLevelDataSettings() { return GetNode<LevelDataSettings>("LevelDataSettings");}

	public override void _Ready()
	{
		levelScene = GetNode<LevelScene>("LevelScene");

		InitGame();
	}
	public void InitGame()
	{
		// Pro moznost benchmarku - tudiz bez playera
		if(WithoutPlayer)
		{
			// Apply Settings
			GameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
			/*
			GameMaster.GM.EnableBlackScreen(false);
			GameMaster.GM.GetLoadingHud().LoadingIsComplete(false);
			*/
		}
		else
		{
			// Apply Settings
			GameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
			GameMaster.GM.EnableBlackScreen(false);
			GameMaster.GM.GetLoadingHud().LoadingIsComplete(false);

            StartGame();
        }

		//StartGame();
	}
	public void StartGame()
	{
		// Emit Signal StartGame
		GameMaster.GM.GetMasterSignals().EmitSignal(MasterSignals.SignalName.GameStart);
	}

}
