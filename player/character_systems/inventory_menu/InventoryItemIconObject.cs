using Godot;
using System;

[Tool]
public partial class InventoryItemIconObject : Panel
{
	[Export] public bool _showLabelID { get {return showLabelID;} set {SetShowLabelID(value); } }
	private bool showLabelID = false;
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

	private void SetShowLabelID(bool newValue)
	{
		showLabelID = newValue;
        GetNode<Label>("LabelID").Visible = newValue;
    }
}
