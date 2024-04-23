using Godot;
using System;

public partial class PlayerStart : Node3D
{
	public enum EPlayerCharacterType {InventoryOld, BaseNew, MoveAnimNew, ActionNew}
	[Export] public bool PlayerStartEnable = true;

	// Which type of player character spawn ?
	[Export] public EPlayerCharacterType SpawnCharacterType = EPlayerCharacterType.InventoryOld;

	// Node contains meshes, which we need visible in editor and unvisible in the game
	public Node3D EditorMesh = null;

	// spawn delay timer
	Godot.Timer spawn_timer = null;

	public override void _Ready()
	{
	}

	public void SpawnPlayerByType(EPlayerCharacterType newPlayerCharacterType)
	{

		switch (newPlayerCharacterType)
		{
			case EPlayerCharacterType.InventoryOld:
				{
					SpawnPlayerInventory();
					break;
				}
			case EPlayerCharacterType.BaseNew:
				{
					SpawnNewCharacterBase();
					break;
				}
            case EPlayerCharacterType.MoveAnimNew:
                {
                    SpawnNewCharacterMoveAnim();
                    break;
                }
            case EPlayerCharacterType.ActionNew:
                {
                    SpawnNewCharacterActionNew();
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
		Node level = CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene();
		if (level == null)
		{
			// If worldlevel for spawn dont finded
			CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.ERROR,
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
			if(CGameMaster.GM.GetGame().GetLevelLoader().isPrecompiledShaders)
				CGameMaster.GM.GetGame().GetLevelLoader().StartPrecompileShaderProcess();

			// Apply Settings
			CGameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
            CGameMaster.GM.GetDebugHud().ApplyAllMainControls();
            CGameMaster.GM.GetGame().GetDebugPanel().AllLoadSettings();

            CGameMaster.GM.GetUniversal().EnableBlackScreen(false);
		}
	}

	public void SpawnPlayerInventory()
	{
        // Instance from scenefile and cast to specific class
        var objectCamera_Instance = GD.Load<PackedScene>(
            "res://player/character_systems/InventoryObjectCamera.tscn").Instantiate() as InventoryObjectCamera;

        var characterInventory_Instance = (FPSCharacter_Inventory)GD.Load<PackedScene>(
            "res://player/FPSCharacter_Inventory.tscn").Instantiate();

        var objectHands_instance = (ObjectHands)GD.Load<PackedScene>(
            "res://player/character_systems/ObjectHands.tscn").Instantiate();

        // Initial settings - link objectCamera to character
        characterInventory_Instance.objectCamera = objectCamera_Instance;
        characterInventory_Instance.objectHands = objectHands_instance;

        // Spawn to worldlevel node
        Node level = CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene();
        if (level == null)
        {
            // If worldlevel for spawn dont finded
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.ERROR,
                "Not find /root/worldlevel for spawn player");
        }
        else
        {
            // Add childs to worldlevel node (Spawn)
            level.AddChild(objectCamera_Instance);
            level.AddChild(characterInventory_Instance);
            level.AddChild(objectHands_instance);

            // Set Positions for objectCamera,character and objectHands as Player Start GlobalPosition
            objectCamera_Instance.GlobalPosition = GlobalPosition;
            characterInventory_Instance.GlobalPosition = GlobalPosition;
            objectHands_instance.GlobalPosition = GlobalPosition;

            // Set new rotation for objectCamera.NodeRotY (look rotation horizontal)
            Vector3 newRotation = objectCamera_Instance.NodeRotY.Rotation;
            newRotation.Y = GlobalRotation.Y;
            objectCamera_Instance.NodeRotY.Rotation = newRotation;

            // Set new rotation for character just for case
            newRotation = characterInventory_Instance.GlobalRotation;
            newRotation.Y = GlobalRotation.Y;
            characterInventory_Instance.GlobalRotation = newRotation;

            // Set new rotation for objectHands just for case
            newRotation = objectHands_instance.GlobalRotation;
            newRotation.Y = GlobalRotation.Y;
            objectHands_instance.GlobalRotation = newRotation;

            // !!! SHADER PRECOMPILATION PROCESS START !!!
            if (CGameMaster.GM.GetGame().GetLevelLoader().isPrecompiledShaders)
                CGameMaster.GM.GetGame().GetLevelLoader().StartPrecompileShaderProcess();

            // Apply Settings
            //CGameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
            //CGameMaster.GM.GetDebugHud().ApplyAllMainControls();

            CGameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
            //CGameMaster.GM.GetDebugHud().ApplyAllMainControls();
            CGameMaster.GM.GetGame().GetDebugPanel().AllLoadSettings();

            //GameMaster.GM.EnableBlackScreen(false);
        }
    }

    public void SpawnNewCharacterBase()
    {
        // Instance from scenefile and cast to specific class
        var newCharacterInstance = GD.Load<PackedScene>(
            "res://player_character/FpsCharacterBase.tscn").Instantiate() as FpsCharacterBase;


        // Spawn to worldlevel node
        Node level = CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene();
        if (level == null)
        {
            // If worldlevel for spawn dont finded
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.ERROR,
                "Not find /root/worldlevel for spawn player");
        }
        else
        {
            // Add childs to worldlevel node (Spawn)
            level.AddChild(newCharacterInstance);

            newCharacterInstance.GlobalPosition = GlobalPosition;
			newCharacterInstance.GetCharacterLookComponent().RotateStart(GlobalRotation);

            /*
            // !!! SHADER PRECOMPILATION PROCESS START !!!
            if (CGameMaster.GM.GetGame().GetLevelLoader().isPrecompiledShaders)
                CGameMaster.GM.GetGame().GetLevelLoader().StartPrecompileShaderProcess();
			*/

            CGameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
            CGameMaster.GM.GetGame().GetDebugPanel().AllLoadSettings();

            //GameMaster.GM.EnableBlackScreen(false);
        }
    }

    public void SpawnNewCharacterMoveAnim()
    {
        // Instance from scenefile and cast to specific class
        var newCharacterInstance = GD.Load<PackedScene>(
            "res://player_character/FPSCharacterMoveAnim.tscn").Instantiate() as FPSCharacterMoveAnim;


        // Spawn to worldlevel node
        Node level = CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene();
        if (level == null)
        {
            // If worldlevel for spawn dont finded
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.ERROR,
                "Not find /root/worldlevel for spawn player");
        }
        else
        {
            // Add childs to worldlevel node (Spawn)
            level.AddChild(newCharacterInstance);

            newCharacterInstance.GlobalPosition = GlobalPosition;
            newCharacterInstance.GetCharacterLookComponent().RotateStart(GlobalRotation);

            /*
            // !!! SHADER PRECOMPILATION PROCESS START !!!
            if (CGameMaster.GM.GetGame().GetLevelLoader().isPrecompiledShaders)
                CGameMaster.GM.GetGame().GetLevelLoader().StartPrecompileShaderProcess();
			*/

            CGameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
            CGameMaster.GM.GetGame().GetDebugPanel().AllLoadSettings();

            //GameMaster.GM.EnableBlackScreen(false);
        }
    }

    public void SpawnNewCharacterActionNew()
    {
        // Instance from scenefile and cast to specific class
        var newCharacterInstance = GD.Load<PackedScene>(
            "res://player_character/FPSCharacterAction.tscn").Instantiate() as FPSCharacterAction;


        // Spawn to worldlevel node
        Node level = CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene();
        if (level == null)
        {
            // If worldlevel for spawn dont finded
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.ERROR,
                "Not find /root/worldlevel for spawn player");
        }
        else
        {
            // Add childs to worldlevel node (Spawn)
            level.AddChild(newCharacterInstance);

            newCharacterInstance.GlobalPosition = GlobalPosition;
            newCharacterInstance.GetCharacterLookComponent().RotateStart(GlobalRotation);

            /*
            // !!! SHADER PRECOMPILATION PROCESS START !!!
            if (CGameMaster.GM.GetGame().GetLevelLoader().isPrecompiledShaders)
                CGameMaster.GM.GetGame().GetLevelLoader().StartPrecompileShaderProcess();
			*/

            CGameMaster.GM.GetSettings().LoadAndApply_AllGraphicsSettings();
            CGameMaster.GM.GetGame().GetDebugPanel().AllLoadSettings();

            //GameMaster.GM.EnableBlackScreen(false);
        }
    }
}
