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
        GD.Print("spawn button test: ");

        FPSCharacter_Inventory a = GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
        if (a == null) return;

        InventoryObjectCamera invCam = a.GetObjectCamera() as InventoryObjectCamera;
        if (invCam == null) return;

        UniversalFunctions.SpawnGameObjectToWorld(
            GameMaster.GM.LevelLoader.GetActualLevelScene(),
            spawnObjectPath, invCam.GetInventoryItemPutPos().GlobalPosition);
    }
}
