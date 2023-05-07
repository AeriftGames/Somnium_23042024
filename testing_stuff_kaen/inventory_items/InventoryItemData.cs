using Godot;
using System;

public partial class InventoryItemData : Resource
{
    [ExportGroupAttribute("InventoryData")]
    [Export] public string itemName { get; set; } = "default item";
    [Export] public string itemInfoText { get; set; }
    [Export] public Mesh itemMeshPreview { get; set; }
    [Export] public bool rotateInInventoryPreview { get; set; }
    [Export] public bool canUseInHand { get; set; }
    [Export] public bool mustBothHandsOnly { get; set; }
    [Export] public bool canPutToWorld { get; set; }
    [Export] public bool canUse { get; set; }
    [Export] public bool canCombine { get; set; }
    [Export] public bool canInspect { get; set; }
    [Export] public bool canDelete { get;set; }

    [ExportGroupAttribute("WorldObjectData")]
    [Export] public string spawnObjectScenePath { get; set; }
}
