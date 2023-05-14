using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

[Tool]
public partial class InventorySlot : Button
{
	public enum EInventorySlotType { socketInventory,socketPlace,socketAttach}
	[Export] public EInventorySlotType inventorySlotType = EInventorySlotType.socketInventory;
	[Export] public bool _showNameSlot { get { return showNameSlot; } set {SetShowNameSlot(value); } }
	[Export] public string _nameSlotText { get {return nameSlotText; } set {SetNameSlotText(value); } }

	private bool showNameSlot = false;
	private string nameSlotText = "";

	private bool hasItem = false;
	private inventory_menu inventoryMenu = null;

	private InventoryItemData inventoryItemData = null;
	private bool isMouseOver = false;
	[Export] public int id = -999;

	public void Init(inventory_menu newInventoryMenu){inventoryMenu = newInventoryMenu;}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!hasItem) return;

		if (Input.IsActionJustPressed("mouseRightClick") && isMouseOver)
		{
			switch (inventorySlotType)
			{
				case EInventorySlotType.socketInventory:
					inventoryMenu.PutFromInventory(this);
					break;
				case EInventorySlotType.socketPlace:
                    inventoryMenu.PutFromInventory(this);
                    break;
				case EInventorySlotType.socketAttach:
					inventoryMenu.DeatachItemAsHotkey(this);
					break;
				default:
					break;
			}

		}
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
		GetNode<InventoryItemIconObject>("InventoryItemIconObject").EnableItemData(newInventoryItemData);
		inventoryItemData = newInventoryItemData;
		hasItem = true;
	}

	public void DestroyUIItem()
	{
        GetNode<InventoryItemIconObject>("InventoryItemIconObject").DisableItemData();
		inventoryItemData = null;
		hasItem = false;

		GD.Print("Destroy ui item");
	}

	public InventoryItemData GetInventoryItemData(){return inventoryItemData;}
	public void _on_pressed(){/*inventoryMenu.FocusUIItem(this)*/;}
	public void _on_mouse_entered(){isMouseOver = true;}
    public void _on_mouse_exited(){isMouseOver = false;}
	public void SetID(int newID){id = newID;}
	public int GetID(){ return id; }
	public bool GetIsMouseHover() { return isMouseOver; }
}
