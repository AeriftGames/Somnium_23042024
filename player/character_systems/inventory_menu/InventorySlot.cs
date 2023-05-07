using Godot;
using System;

[Tool]
public partial class InventorySlot : Button
{
	[Export] public bool _showNameSlot { get { return showNameSlot; } set {SetShowNameSlot(value); } }
	[Export] public string _nameSlotText { get {return nameSlotText; } set {SetNameSlotText(value); } }

	private bool showNameSlot = false;
	private string nameSlotText = "";

	private bool hasItem = false;
	private inventory_menu inventoryMenu = null;

	private InventoryItemData inventoryItemData = null;

	public void Init(inventory_menu newInventoryMenu){inventoryMenu = newInventoryMenu;}

	public void SetShowNameSlot(bool newShow)
	{
		showNameSlot = newShow;
		GetNode<Label>("LabelNameSlot").Visible = newShow;
	}
	public void SetNameSlotText(string newText)
	{
		nameSlotText = newText;
        GetNode<Label>("LabelNameSlot").Text = nameSlotText;
    }

	public bool HasUIItem() { return hasItem; }

	public void SetItem(InventoryItemData newInventoryItemData)
	{
		GetNode<InventoryItemIconObject>("InventoryItemIconObject").EnableItemData(newInventoryItemData);
		inventoryItemData = newInventoryItemData;
		hasItem = true;
	}

	public void DestroyUIItem()
	{
        GetNode<InventoryItemIconObject>("InventoryItemIconObject").DisableItemData();
		inventoryItemData = null;
		hasItem = false;
	}

	public InventoryItemData GetInventoryItemData()
	{
		return inventoryItemData;
	}

	public void _on_pressed()
	{
		inventoryMenu.FocusUIItem(this);
	}
}
