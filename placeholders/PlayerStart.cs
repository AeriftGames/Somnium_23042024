using Godot;
using System;

public partial class PlayerStart : Node3D
{
	public enum EPlayerCharacterType { WalkingEffects, Interaction}

	// Which type of player character spawn ?
	[Export] public EPlayerCharacterType SpawnCharacterType = EPlayerCharacterType.Interaction;

	// Node contains meshes, which we need visible in editor and unvisible in the game
	public Node3D EditorMesh = null;

	// spawn delay timer
	Godot.Timer spawn_timer = null;

	public override void _Ready()
	{
		// Po Startu zneviditelnime a nechame objekt znicit (chceme videt jen v editoru)
		EditorMesh = GetNode<Node3D>("EditorMesh");
		EditorMesh.Visible = false;
		EditorMesh.QueueFree();

		spawn_timer = new Godot.Timer();
		// Create timer for delaying spawn if start game
		var callable_spawn = new Callable(this,"SpawnNewPlayer");
		spawn_timer.Connect("timeout", callable_spawn);
		spawn_timer.WaitTime = 0.2;
		spawn_timer.OneShot = true;
		AddChild(spawn_timer);
		spawn_timer.Start();
	}

	// tohle vola timer po zacatku hry + timer.delay (0,2s)
	public void SpawnNewPlayer()
	{
		SpawnPlayerByType(SpawnCharacterType);
	}

	public void SpawnPlayerByType(EPlayerCharacterType newPlayerCharacterType)
	{

		switch (newPlayerCharacterType)
		{
			case EPlayerCharacterType.WalkingEffects:
				{
					break;
				}
			case EPlayerCharacterType.Interaction:
				{
					SpawnPlayerInteraction();
					break;
				}
			default:
				break;
		}
	}

	public void SpawnPlayerInteraction()
	{
		// Instance from scenefile and cast to specific class
		var objectCamera_Instance = (ObjectCamera)GD.Load<PackedScene>(
			"res://player/character_systems/ObjectCamera.tscn").Instantiate();
		
		var characterInteraction_Instance = (FPSCharacter_Interaction)GD.Load<PackedScene>(
			"res://player/FPSCharacter_Interaction.tscn").Instantiate();
		
		var objectHands_instance = (ObjectHands)GD.Load<PackedScene>(
			"res://player/character_systems/ObjectHands.tscn").Instantiate();
		
		// Initial settings - link objectCamera to character
		characterInteraction_Instance.objectCamera = objectCamera_Instance;
		characterInteraction_Instance.objectHands = objectHands_instance;

		// Spawn to worldlevel node
		Node level = GetNode("/root/worldlevel");
		if (level == null)
		{
			// If worldlevel for spawn dont finded
			GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.ERROR,
				"Not find /root/worldlevel for spawn player");
		}
		else
		{
			// Add childs to worldlevel node (Spawn)
			level.AddChild(objectCamera_Instance);
			level.AddChild(characterInteraction_Instance);
			level.AddChild(objectHands_instance);

			// Set Positions for objectCamera,character and objectHands as Player Start GlobalPosition
			objectCamera_Instance.GlobalPosition = GlobalPosition;
			characterInteraction_Instance.GlobalPosition = GlobalPosition;
			objectHands_instance.GlobalPosition = GlobalPosition;

			// Set new rotation for objectCamera.NodeRotY (look rotation horizontal)
			Vector3 newRotation = objectCamera_Instance.NodeRotY.Rotation;
			newRotation.Y = GlobalRotation.Y;
			objectCamera_Instance.NodeRotY.Rotation = newRotation;

			// Set new rotation for character just for case
			newRotation = characterInteraction_Instance.GlobalRotation;
			newRotation.Y = GlobalRotation.Y;
			characterInteraction_Instance.GlobalRotation = newRotation;

			// Set new rotation for objectHands just for case
			newRotation = objectHands_instance.GlobalRotation;
			newRotation.Y = GlobalRotation.Y;
			objectHands_instance.GlobalRotation = newRotation;
			
			// !!! SHADER PRECOMPILATION PROCESS START !!!
			if(GameMaster.GM.LevelLoader.isPrecompiledShaders)
				GameMaster.GM.LevelLoader.StartPrecompileShaderProcess();

			// Apply Settings
			GameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
			GameMaster.GM.GetDebugHud().ApplyAllMainControls();

			GameMaster.GM.EnableBlackScreen(false);
			//delete
			spawn_timer.Stop();
			spawn_timer.QueueFree();
		}
	}
}
