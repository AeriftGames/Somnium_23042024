using Godot;
using System;

public partial class InventoryItemIconObject : Panel
{
	public void EnableItemData(InventoryItemData newInventoryItemData)
	{
		GetNode<Label>("ItemName").Text = newInventoryItemData.itemName;
		Visible = true;
	}

	public void DisableItemData()
	{
		GetNode<Label>("ItemName").Text = "";
        Visible = false;
    }
}
