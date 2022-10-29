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
    Godot.Timer spawn_timer = new Godot.Timer();

    public override void _Ready()
	{
		// Po Startu zneviditelnime a nechame objekt znicit (chceme videt jen v editoru)
		EditorMesh = GetNode<Node3D>("EditorMesh");
		EditorMesh.Visible = false;
		EditorMesh.QueueFree();

        // Create timer for delaying spawn if start game
        var callable_spawn = new Callable(SpawnNewPlayer);
        spawn_timer.Connect("timeout", callable_spawn);
        spawn_timer.WaitTime = 0.2;
        spawn_timer.OneShot = true;
        AddChild(spawn_timer);
		spawn_timer.Start();
    }

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
					// Instance from scenefile and cast to specific class
					var objectCamera_Instance = (ObjectCamera)GD.Load<PackedScene>(
                        "res://player/character_systems/ObjectCamera.tscn").Instantiate();

					var characterInteraction_Instance = (FPSCharacter_Interaction)GD.Load<PackedScene>(
                        "res://player/FPSCharacter_Interaction.tscn").Instantiate();

                    // Initial settings - link objectCamera to character
                    characterInteraction_Instance.objectCamera = objectCamera_Instance;

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

						// Set Positions for objectCamera and character as Player Start GlobalPosition
						objectCamera_Instance.GlobalPosition = GlobalPosition;
						characterInteraction_Instance.GlobalPosition = GlobalPosition;

						// Set new rotation for objectCamera.NodeRotY (look rotation horizontal)
						Vector3 newRotation = objectCamera_Instance.NodeRotY.Rotation;
						newRotation.y = Rotation.y;
						objectCamera_Instance.NodeRotY.Rotation = newRotation;

                        // Set new rotation for character just for case
                        newRotation = characterInteraction_Instance.GlobalRotation;
                        newRotation.y = GlobalRotation.y;
                        characterInteraction_Instance.GlobalRotation = newRotation;
                    }

                    break;
				}
			default:
				break;
		}
	}
}
