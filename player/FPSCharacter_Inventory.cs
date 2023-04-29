using Godot;
using System;
using System.Linq.Expressions;

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
        // vytvorime (spawn) DeadCamBody do levelu na misto kde se aktualne nachazela kamera
        dead_cam_body dcb = 
            GD.Load<PackedScene>("res://player/character_systems/dead_cam_body.tscn").Instantiate() as dead_cam_body;
        GameMaster.GM.LevelLoader.GetActualLevelScene().AddChild(dcb);

        // aktivujeme DeadCam
        dcb.ActivateDeadCam();
        dcb.ApplyCentralImpulse(GetLastMotion()*50f);
    }
}
