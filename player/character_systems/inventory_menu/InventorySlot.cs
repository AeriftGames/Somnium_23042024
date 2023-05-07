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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

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
		GetInventoryIconObject().EnableItemData(newInventoryItemData);
		hasItem = true;
	}

	public void DestroyUIItem()
	{
		GetInventoryIconObject().DisableItemData();
		hasItem = false;
	}

	private InventoryItemIconObject GetInventoryIconObject()
	{
		return GetNode<InventoryItemIconObject>("InventoryItemIconObject");
	}
}
