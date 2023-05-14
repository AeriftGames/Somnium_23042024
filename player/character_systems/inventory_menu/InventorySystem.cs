using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class InventorySystem : Node
{
    [Export] public int MaxInventoryCapacity = 15;
    [Export] public Array<InventoryItemData> inventoryItems = new Array<InventoryItemData>();

	private FPSCharacter_Inventory inventoryCharacter = null;

	public override void _Ready()
	{
    }

	public override void _Process(double delta)
	{
	}

	public void Init(FPSCharacter_Inventory ownCharacter)
	{
		inventoryCharacter = ownCharacter;

        SetMaxInventoryCapacity(MaxInventoryCapacity);
    }

	public bool AddItemToInventory(InventoryItemData newInventoryItemData)
	{
		// mame vubec volno v inventari ? jestli ne opustime funkci
		if(!HasInventoryFreeSlot())
			return false;

		// udela kopii naseho item data (prerusi vazby resource souboru
		InventoryItemData copiedItemData = newInventoryItemData.Duplicate() as InventoryItemData;

        // nastavi inventory item data prvni volny (slot id) a prida item do inventare, pokud je -1 = neni misto
        copiedItemData.InventoryHoldingSlotID = inventoryCharacter.GetInventoryMenu().GetFirstFreeSlotInInventory();
		if (copiedItemData.InventoryHoldingSlotID == -1) return false;

        inventoryItems.Add(copiedItemData);

		// prida ui na dany slot
		inventoryCharacter.GetInventoryMenu().AddItemToSlot(copiedItemData,
            copiedItemData.InventoryHoldingSlotID);

        GD.Print("item byl uspesne pridan do inventare");
        return true;
	}

    public Array<InventoryItemData> GetAllInventoryItems() { return inventoryItems; }

	public bool HasInventoryFreeSlot()
	{
		return GetAllInventoryItems().Count < MaxInventoryCapacity ? true : false;
	}

	public bool PutItemFromInventoryToWorld(InventoryItemData newInventoryItemData)
	{
		InventoryObjectCamera invCam = inventoryCharacter.objectCamera as InventoryObjectCamera;
		if (invCam == null) return false;

        // Spawn
        Node3D itemPut = GD.Load<PackedScene>(newInventoryItemData.spawnObjectScenePath).Instantiate() as Node3D;
		if(itemPut != null)
		GameMaster.GM.LevelLoader.GetActualLevelScene().AddChild(itemPut);
        itemPut.GlobalPosition = invCam.GetInventoryItemPutPos().GlobalPosition;

        // Destroy from InventorySystem
        GetAllInventoryItems().Remove(newInventoryItemData);

        // nastaveni pro informaci ze se item nachazi mimo inventar, tedy ve svete
        newInventoryItemData.InventoryHoldingSlotID = -1;

        return true;
	}

	public void SetMaxInventoryCapacity(int newMaxCapacity)
	{
		MaxInventoryCapacity = newMaxCapacity;
	}
}
