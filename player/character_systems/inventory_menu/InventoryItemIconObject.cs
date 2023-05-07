using Godot;
using System;

public partial class InventoryItemIconObject : Panel
{

	public void EnableItemData(InventoryItemData newInventoryItemData)
	{
		GetNode<Label>("ItemName").Text = newInventoryItemData.itemName;
		Visible = true;

		GetNode<InventoryItemPreview>("SubViewportContainer").Activate(newInventoryItemData.itemMeshPreview);
	}

	public void DisableItemData()
	{
		GetNode<Label>("ItemName").Text = "";
        Visible = false;
		GetNode<InventoryItemPreview>("SubViewportContainer").Deactivate();
    }
}
