using Godot;
using System;

public partial class DragAndDropItem : Panel
{
    private bool isDragUpdate = false;
    private InventoryItemData inventoryItemData;

    public void Init(InventoryItemData newInventoryItemData)
    {
        GetNode<Label>("ItemName").Text = newInventoryItemData.itemName;
        GetNode<Label>("ItemName").Visible = newInventoryItemData.showNameInSlot;

        testing_render_inventory_items.ApplyItemSubViewportSetting(
            GetNode<SubViewport>("SubViewportContainer/SubViewport"),
            newInventoryItemData.SettingsForSlot,
            newInventoryItemData.itemMeshPreview);

        GetNode<InventoryItemPreview>("SubViewportContainer").Activate();

        SetGlobalPosition(GetGlobalMousePosition());

        inventoryItemData = newInventoryItemData;

        Visible = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!isDragUpdate) return;

        SetGlobalPosition(GetGlobalMousePosition());
    }

    public void Destroy()
    {
        Visible = false;
        QueueFree();
    }

    public void SetDragUpdate(bool newUpdate)
    {
        isDragUpdate = newUpdate;
    }

    public InventoryItemData GetInventoryItemData() { return inventoryItemData; }
}
