using Godot;
using Godot.Collections;
using System;

public partial class InventorySystem : Node
{
	[Export] public Array<InventoryItemData> inventoryItems = new Array<InventoryItemData>();

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public bool AddItemToInventory(InventoryItemData newInventoryItemData)
	{
		inventoryItems.Add(newInventoryItemData);
        GD.Print("item byl uspesne pridan do inventare");

        return true;
	}

    public Array<InventoryItemData> GetAllInventoryItems() { return inventoryItems; }

	public bool PutItemFromInventoryToWorld(InventoryItemData newInventoryItemData)
	{
		FPSCharacter_Inventory charInventory = GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
		if (charInventory == null) return false;

		InventoryObjectCamera invCam = charInventory.objectCamera as InventoryObjectCamera;
		if (invCam == null) return false;

        // Spawn
        Node3D itemPut = GD.Load<PackedScene>(newInventoryItemData.spawnObjectScenePath).Instantiate() as Node3D;
		if(itemPut != null)
		GameMaster.GM.LevelLoader.GetActualLevelScene().AddChild(itemPut);
        itemPut.GlobalPosition = invCam.GetInventoryItemPutPos().GlobalPosition;

        // Destroy from InventorySystem
        GetAllInventoryItems().Remove(newInventoryItemData);

		return true;
	}
}
