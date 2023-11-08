using Godot;
using System;

[Tool]
public partial class testing_render_inventory_items : Control
{
    [Export] public bool _updateNow { get { return updateNow; } set {UpdateNow(value); } }
    private bool updateNow = false;

    [Export] public bool _savePreviewAndSlotSceneNow { get { return savePreviewAndSlotSceneNow; } set { SavePreviewAndSlotSceneNow(value); } }
    private bool savePreviewAndSlotSceneNow = false;

    [Export] public InventoryItemData inventoryItemData = new InventoryItemData();

    MeshInstance3D itemPreviewMesh = null;
    bool isRotation = false;
    float speedRotation = 1f;

    private void UpdateNow(bool newSave)
    {
        if(inventoryItemData == null) { GD.Print("nenalezeno InventoryItemData"); return; } 
        GD.Print("UPDATE");

        // texty
        GetNode<Label>("Panel/Label").Text = inventoryItemData.itemName;
        GetNode<RichTextLabel>("Panel/RichTextLabel").Text = inventoryItemData.itemInfoText;

        GetNode<Label>("InventoryItemIconObject/ItemName").Text = inventoryItemData.itemName;
        GetNode<Label>("InventoryItemIconObject/ItemName").Visible = inventoryItemData.showNameInSlot;

        // aplikovani nastaveni pro sceny preview a slots
        ApplySettingsForScenes();

        Visible = true;

        updateNow = false;
    }

    public void ApplySettingsForScenes()
    {
        ApplyItemSubViewportSetting(GetNode<SubViewport>("Panel_ItemPreview/SubViewportContainer/SubViewport"),
            inventoryItemData.SettingsForPreview,inventoryItemData.itemMeshPreview);

        ApplyItemSubViewportSetting(GetNode<SubViewport>("InventoryItemIconObject/SubViewportContainer/SubViewport"),
            inventoryItemData.SettingsForSlot,inventoryItemData.itemMeshPreview);
    }
    public void SavePreviewAndSlotSceneNow(bool newSave)
    {
        if (inventoryItemData == null) { GD.Print("nenalezeno InventoryItemData"); return; }
        savePreviewAndSlotSceneNow = false;
        GD.Print("SAVE PREVIEW AND SLOT SCENE NOW");

        Camera3D cam = GetNode<Camera3D>("Panel_ItemPreview/SubViewportContainer/SubViewport/Camera3D");
        OmniLight3D light = GetNode<OmniLight3D>("Panel_ItemPreview/SubViewportContainer/SubViewport/OmniLight3D");
        MeshInstance3D mesh = GetNode<MeshInstance3D>("Panel_ItemPreview/SubViewportContainer/SubViewport/MeshInstance3D");

        inventoryItemData.itemMeshPreview = mesh.Mesh;
        // for preview
        inventoryItemData.SettingsForPreview.cameraPos = cam.Position;
        inventoryItemData.SettingsForPreview.cameraFov = cam.Fov;

        inventoryItemData.SettingsForPreview.lightPos = light.Position;
        inventoryItemData.SettingsForPreview.lightPower = light.LightEnergy;
        inventoryItemData.SettingsForPreview.lightSize = light.LightSize;
        inventoryItemData.SettingsForPreview.lightRange = light.OmniRange;
        inventoryItemData.SettingsForPreview.lightColor = light.LightColor;

        inventoryItemData.SettingsForPreview.meshPos = mesh.Position;
        inventoryItemData.SettingsForPreview.meshRot = mesh.RotationDegrees;
        inventoryItemData.SettingsForPreview.meshScale = mesh.Scale;

        // for slot
        cam = GetNode<Camera3D>("InventoryItemIconObject/SubViewportContainer/SubViewport/Camera3D");
        light = GetNode<OmniLight3D>("InventoryItemIconObject/SubViewportContainer/SubViewport/OmniLight3D");
        mesh = GetNode<MeshInstance3D>("InventoryItemIconObject/SubViewportContainer/SubViewport/MeshInstance3D");

        inventoryItemData.SettingsForSlot.cameraPos = cam.Position;
        inventoryItemData.SettingsForSlot.cameraFov = cam.Fov;

        inventoryItemData.SettingsForSlot.lightPos = light.Position;
        inventoryItemData.SettingsForSlot.lightPower = light.LightEnergy;
        inventoryItemData.SettingsForSlot.lightSize = light.LightSize;
        inventoryItemData.SettingsForSlot.lightRange = light.OmniRange;
        inventoryItemData.SettingsForSlot.lightColor = light.LightColor;

        inventoryItemData.SettingsForSlot.meshPos = mesh.Position;
        inventoryItemData.SettingsForSlot.meshRot = mesh.RotationDegrees;
        inventoryItemData.SettingsForSlot.meshScale = mesh.Scale;
    }

    static public void ApplyItemSubViewportSetting(SubViewport subviewport,ItemSubViewportSetting setting, Mesh newMesh)
    {
        Camera3D cam = subviewport.GetNode<Camera3D>("Camera3D");
        OmniLight3D light = subviewport.GetNode<OmniLight3D>("OmniLight3D");
        MeshInstance3D mesh = subviewport.GetNode<MeshInstance3D>("MeshInstance3D");

        cam.Position = setting.cameraPos;
        cam.Fov = setting.cameraFov;

        light.Position = setting.lightPos;
        light.LightEnergy = setting.lightPower;
        light.LightSize = setting.lightSize;
        light.OmniRange = setting.lightRange;
        light.LightColor = setting.lightColor;

        mesh.Mesh = newMesh;

        mesh.Position = setting.meshPos;
        mesh.RotationDegrees = setting.meshRot;
        mesh.Scale = setting.meshScale;
    }
}
