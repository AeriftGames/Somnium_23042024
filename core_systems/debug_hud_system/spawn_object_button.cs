using Godot;
using System;

public partial class spawn_object_button : Button
{
    private string spawnObjectPath = "";
    private string spawnObjectName = "";

    public void SetSpawnObjectData(string newSpawnObjectPath,string newSpawnObjectName)
    {
        spawnObjectPath = newSpawnObjectPath;
        spawnObjectName = newSpawnObjectName;
    }

    public string GetSpawnObjectPath() { return spawnObjectPath; }

	public void _on_pressed()
	{
        //OldCharacterSpawnObject();
        NewSpawnObject();
    }

    private void OldCharacterSpawnObject()
    {
        FPSCharacter_Inventory a = CGameMaster.GM.GetGame().GetFPSCharacterOld() as FPSCharacter_Inventory;
        if (a == null) return;

        InventoryObjectCamera invCam = a.GetObjectCamera() as InventoryObjectCamera;
        if (invCam == null) return;

        Godot.Collections.Array<Node3D> allSpawnNodes = UniversalFunctions.SpawnGameObjectToWorld(
            CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene(),
            spawnObjectPath, invCam.GetInventoryItemPutPos().GlobalPosition,
            CGameMaster.GM.GetDebugHud().GetNeedNumOfSpawn());

        GD.Print("Spawn " + allSpawnNodes.Count + " object of: " + allSpawnNodes[0].Name);
    }
    private void NewSpawnObject()
    {
        FPSCharacterAction charAction = CGameMaster.GM.GetGame().GetFPSCharacterBase() as FPSCharacterAction;
        if (charAction == null) return;

        Godot.Collections.Array<Node3D> allSpawnNodes = UniversalFunctions.SpawnGameObjectToWorld(
            CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene(),
            spawnObjectPath, charAction.GetCharacterLookComponent().GetSpawnItemPoint().GlobalPosition,
            CGameMaster.GM.GetDebugHud().GetNeedNumOfSpawn());

        GD.Print("Spawn " + allSpawnNodes.Count + " object of: " + allSpawnNodes[0].Name);
    }
}
