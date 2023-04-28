using Godot;
using System;
using System.Linq.Expressions;

public partial class FPSCharacter_Inventory : FPSCharacter_Interaction
{
    public bool isDead = false;

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

        // Move Dead Camera to floor
        if(isDead)
        {
            GD.Print("sdasdasdsad");

            GetObjectCamera().GlobalPosition = GetObjectCamera().GlobalPosition.MoveToward(
                UniversalFunctions.GetNodeUpVector(GetObjectCamera())*10,
                (float)delta);
        }
    }

    public inventory_menu GetInventoryMenu(){ return ourInventoryMenu; }

    public override void EventDead(ECharacterReasonDead newReasonDead, string newAdditionalData = "", 
        bool newPrintToConsole = false)
    {
        base.EventDead(newReasonDead, newAdditionalData, newPrintToConsole);

        switch (newReasonDead)
        {
            case ECharacterReasonDead.NoHealth:
                ApplyNoHealthEffect();
                break;
            case ECharacterReasonDead.FallFromHeight:
                break;
            case ECharacterReasonDead.KilledFromEnemy:
                break;
            default:
                break;
        }
    }

    private void ApplyNoHealthEffect()
    {
        // vypne lerping kamery k characteru
        GetObjectCamera().SetLerpToCharacterEnable(false);
        isDead = true;
    }
}
