using Godot;
using System;

public partial class InventoryItemIconObject : Panel
{
	public void EnableItemData(InventoryItemData newInventoryItemData)
	{
		GetNode<Label>("ItemName").Text = newInventoryItemData.itemName;
		GetNode<Label>("ItemName").Visible = newInventoryItemData.showNameInSlot;

		testing_render_inventory_items.ApplyItemSubViewportSetting(
			GetNode<SubViewport>("SubViewportContainer/SubViewport"),
			newInventoryItemData.SettingsForSlot,
			newInventoryItemData.itemMeshPreview);

		GetNode<InventoryItemPreview>("SubViewportContainer").Activate();
		GetNode<Label>("LabelID").Text = newInventoryItemData.InventoryHoldingSlotID.ToString();

        Visible = true;
    }

	public void DisableItemData()
	{
		GetNode<Label>("ItemName").Text = "";
        Visible = false;
		GetNode<InventoryItemPreview>("SubViewportContainer").Deactivate();
    }
}
