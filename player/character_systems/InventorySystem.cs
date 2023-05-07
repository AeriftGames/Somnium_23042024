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
}
