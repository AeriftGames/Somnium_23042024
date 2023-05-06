using Godot;
using System;
using System.Linq.Expressions;

public partial class FPSCharacter_Inventory : FPSCharacter_Interaction
{
    private CharacterInfoHud characterInfoHud = null;
    private inventory_menu ourInventoryMenu = null;

    private HealthSystem healthSystem = null;

    [ExportGroupAttribute("HealthSystem")]
    [Export] public float StartActualHealth = 100.0f;
    [Export] public float StartMaxHealth = 100.0f;
    [Export] public float StartHealthRegenVal = 1.0f;
    [Export] public float StartHealthRegenTick = 0.5f;
    [Export] public bool StartHealthRegenEnable = false;

    [ExportGroupAttribute("DamageAndDeath")]
    [Export] public Godot.Collections.Array<AudioStream> HurtAudios;
    [Export] public Godot.Collections.Array<AudioStream> DeathAudios;
    [Export] public Godot.Collections.Array<AudioStream> BodyFallAudios;

    AudioStreamPlayer hurtPlayer = null;

    private dead_cam_body deadCamBody = null;

    public override void _Ready()
    {
        base._Ready();

        //
        characterInfoHud = GetNode<CharacterInfoHud>("AllHuds/CharacterInfoHud");
        ourInventoryMenu = GetNode<inventory_menu>("AllHuds/InventoryMenu");

        // vytvori healthSystem a nastavime aktualni(startovni) hodnoty
        healthSystem = new HealthSystem(this);
        healthSystem.SetAllData(StartActualHealth,StartMaxHealth,StartHealthRegenVal,StartHealthRegenTick,
            StartHealthRegenEnable);

        hurtPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Hurts");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // opem inventory menu
        if(IsInputEnable() && !GetInventoryMenu().GetActive() && Input.IsActionJustPressed("toggleInventory"))
            GetInventoryMenu().SetActive(true);

        if (IsInputEnable() && Input.IsActionJustPressed("test_damage"))
            GetHealthSystem().RemoveHealth(10);

        if (IsInputEnable() && Input.IsActionJustPressed("test_regen"))
            GetHealthSystem().AddHealth(10);
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
        deadCamBody = 
            GD.Load<PackedScene>("res://player/character_systems/dead_cam_body.tscn").Instantiate() as dead_cam_body;
        GameMaster.GM.LevelLoader.GetActualLevelScene().AddChild(deadCamBody);

        // aktivujeme DeadCam
        deadCamBody.ActivateDeadCam();
        deadCamBody.ApplyCentralImpulse(GetLastMotion()*50f);
    }

    public HealthSystem GetHealthSystem() { return healthSystem; }
    public CharacterInfoHud GetCharacterInfoHud() { return characterInfoHud; }
    public Godot.Collections.Array<AudioStream> GetHurtAudios(){return HurtAudios;}
    public Godot.Collections.Array<AudioStream> GetDeathAudios() { return DeathAudios;}
    public Godot.Collections.Array<AudioStream> GetBodyFallAudios() { return BodyFallAudios; }

    public AudioStreamPlayer GetHurtPlayer() { return hurtPlayer; }

    public override void FreeAll()
    {
        base.FreeAll();
        //deadCamBody.FreeAll();
    }
}
