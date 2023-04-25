using Godot;
using System;

public partial class FPSCharacter_Inventory : FPSCharacter_Interaction
{
    private inventory_menu ourInventoryMenu = null;

    public override void _Ready()
    {
        base._Ready();

        ourInventoryMenu = GetNode<inventory_menu>("AllHuds/InventoryMenu");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // opem inventory menu
        if(IsInputEnable() && !GetInventoryMenu().GetActive() && Input.IsActionJustPressed("toggleInventory"))
                GetInventoryMenu().SetActive(true);
    }

    public inventory_menu GetInventoryMenu(){ return ourInventoryMenu; }
}
