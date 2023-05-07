using Godot;
using System;

public partial class InventoryItemDataNode : Node3D
{
	[Export] public InventoryItemData Data = new InventoryItemData();
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UseWantAddToInventory(string itemForSpawn_sceneFilePath)
	{
		//GD.Print("add to inventory: scenefilepath: "+itemForSpawn_sceneFilePath);

		FPSCharacter_Inventory charInventory = GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
		if (charInventory == null) return;

		// pokud neni vyplnena manualne cesta k souboru ktery se ma spawnout pri PUT TO WORLD,
		// nastavime ho automaticky z parenta
		if(Data.spawnObjectScenePath != "")
			Data.spawnObjectScenePath = itemForSpawn_sceneFilePath;

		// pokusime se pridat item do inventare hrace
		charInventory.GetInventorySystem().AddItemToInventory(Data);
	}
}
