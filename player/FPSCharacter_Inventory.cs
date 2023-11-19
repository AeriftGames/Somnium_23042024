using Godot;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

public partial class FPSCharacter_Inventory : FPSCharacter_Interaction
{
    private CharacterInfoHud characterInfoHud = null;
    private inventory_menu ourInventoryMenu = null;

    private CharacterHealthComponent characterHealthComponent = null;
    private CharacterStaminaComponent characterStaminaComponent = null;
    private CharacterInventoryComponent characterInventoryComponent = null;
    private CharacterStairsComponent characterStairsComponent = null;
    private CharacterShootBallComponet characterShootBallComponet = null;
    private CharacterUseLadderComponent characterUseLadderComponent = null;

    [ExportGroupAttribute("DamageAndDeath")]
    [Export] public Godot.Collections.Array<AudioStream> HurtAudios;
    [Export] public Godot.Collections.Array<AudioStream> DeathAudios;
    [Export] public Godot.Collections.Array<AudioStream> BodyFallAudios;

    AudioStreamPlayer hurtPlayer = null;
    AudioStreamPlayer universalPlayer = null;

    private dead_cam_body deadCamBody = null;

    public override void _Ready()
    {
        base._Ready();

        //
        characterInfoHud = GetNode<CharacterInfoHud>("AllHuds/CharacterInfoHud");
        ourInventoryMenu = GetNode<inventory_menu>("AllHuds/InventoryMenu");
        
        // health component
        characterHealthComponent = GetNode<CharacterHealthComponent>("CharacterComponents/CharacterHealthComponent");
        characterHealthComponent.StartInit(this);

        // stamina component
        characterStaminaComponent = GetNode<CharacterStaminaComponent>("CharacterComponents/CharacterStaminaComponent");
        characterStaminaComponent.StartInit(this);

        // inventory component
        characterInventoryComponent = GetNode<CharacterInventoryComponent>("CharacterComponents/CharacterInventoryComponent");
        characterInventoryComponent.StartInit(this);

        // stairs component
        characterStairsComponent = GetNode<CharacterStairsComponent>("CharacterComponents/CharacterStairsComponent");
        characterStairsComponent.StartInit(this);

        // shootball component
        characterShootBallComponet = GetNode<CharacterShootBallComponet>("CharacterComponents/CharacterShootBallComponet");
        characterShootBallComponet.StartInit(this);

        // use ladder component
        characterUseLadderComponent = GetNode<CharacterUseLadderComponent>("CharacterComponents/CharacterUseLadderComponent");
        characterUseLadderComponent.StartInit(this);

        hurtPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Hurts");
        universalPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Universal");

        // inits
        ourInventoryMenu.Init(GetInventoryComponent());
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // opem inventory menu
        if(IsInputEnable() && !GetInventoryMenu().GetActive() && Input.IsActionJustPressed("toggleInventory"))
            GetInventoryMenu().SetActive(true);

        if (IsInputEnable() && Input.IsActionJustPressed("test_damage"))
            GetCharacterHealthComponent().RemoveHealth(10);

        if (IsInputEnable() && Input.IsActionJustPressed("test_regen"))
            GetCharacterHealthComponent().AddHealth(10);
        
        // STAMINA Process logic *******************************************************

        // for sprint
        if(GetCharacterStaminaComponent().ActiveStaminaForSprint)
        {
            if (CanSprint && GetIsSprint() && ActualMovementSpeed > 5.0f)
            {
                GetCharacterStaminaComponent().RemoveStamina(0.15f);
                GetCharacterStaminaComponent().SetStaminaRegenVal(0.0f);
            }
            else
            {
                GetCharacterStaminaComponent().SetStaminaRegenVal(1.0f);
            }

            if (GetCharacterStaminaComponent().GetStamina() <= 1.0f)
                CanSprint = false;
            else if(GetCharacterStaminaComponent().GetStamina() > 10.0f)
                CanSprint = true;
        }

        // for fast stamina regen standing
        if(GetCharacterStaminaComponent().ActiveFastRegenForStanding)
        {
            // stojime na miste ?
            if (ActualMovementSpeed <= 1.0f)
                GetCharacterStaminaComponent().SetStaminaRegenTick(0.05f);
            else
                GetCharacterStaminaComponent().SetStaminaRegenTick(0.2f);
        }

        // for fast stamina regen crouching
        if (GetCharacterStaminaComponent().ActiveFastRegenForCrouching)
        {
            // jsme skrceni ?
            if (GetCharacterPosture() == ECharacterPosture.Crunch)
                GetCharacterStaminaComponent().SetStaminaRegenTick(0.085f);
        }

        // put inventory item to world
        TestInventoryItemPutToWorld();
    }

    public void TestInventoryItemPutToWorld()
    {
        // put inventory item to world
        if (GetInventoryComponent().wantPutItem)
        {
            //
            InventoryObjectCamera invObjectCam = GetObjectCamera() as InventoryObjectCamera;
            if (invObjectCam == null) return;

            Vector3 resA = invObjectCam.GetInventoryItemPutPos().GlobalPosition;
            Vector3 resB = invObjectCam.GetInventoryItemPutPos().GlobalPosition;

            // A - testujeme kolizi s layer 1 = level collisions
            // zjisteni bezpecne pozice pro vypusteni itemu
            UniversalFunctions.HitResult hit = UniversalFunctions.IsSimpleRaycastHit(this,
                GetFPSCharacterCamera().GlobalPosition,
                invObjectCam.GetInventoryItemPutPos().GlobalPosition, 1);

            Vector3 spawnPos = invObjectCam.GetInventoryItemPutPos().GlobalPosition;

            if (hit.isHit)
                resA = hit.HitPosition;

            // B - testujeme kolizi s layer 8 = physics items
            // zjisteni bezpecne pozice pro vypusteni itemu
            UniversalFunctions.HitResult hit2 = UniversalFunctions.IsSimpleRaycastHit(this,
                GetFPSCharacterCamera().GlobalPosition,
                invObjectCam.GetInventoryItemPutPos().GlobalPosition, 8);

            if (hit2.isHit)
                resB = hit2.HitPosition;

            if(resA.DistanceTo(GetFPSCharacterCamera().GlobalPosition) < resB.DistanceTo(GetFPSCharacterCamera().GlobalPosition))
                spawnPos = resA;
            else
                spawnPos = resB;

            GetInventoryComponent().PutItemFromInventoryToWorld(GetInventoryComponent().wantInventoryItemDataPutToWorld,
                spawnPos);
            GetInventoryComponent().ResetWantItemPutFromInventory();
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

    // override for STAMINA proccess
    public override void EventJumping()
    {
        // mame aktivovany stamina system pro skok ?
        if(GetCharacterStaminaComponent().ActiveStaminaForJump)
        {
            // mame dostatek staminy pro skok ?
            if (GetCharacterStaminaComponent().GetStamina() >= 7.0f)
            {
                // ubereme staminu a provedeme skok
                GetCharacterStaminaComponent().RemoveStamina(7.0f);
                base.EventJumping();
            }
        }
        else // pokud ne tak pouzijeme skok vzdy a neovlivnujeme staminu
        {
            base.EventJumping();
        }
    }

    // override for STAMINA proccess
    public override void EventLandingEffect(float heightfall)
    {
        base.EventLandingEffect(heightfall);

        if(GetCharacterStaminaComponent().ActiveStaminaForLand)

        if (heightfall < 0.15f)
        {
            // very mini
            GetCharacterStaminaComponent().RemoveStamina(3.0f);
        }
        else if (heightfall <= MiniHeightForLandingEffect)
        {
            // mini land
            GetCharacterStaminaComponent().RemoveStamina(5.0f);
        }
        else if (heightfall <= SmallHeightForLandingEffect)
        {
            // small land
            GetCharacterStaminaComponent().RemoveStamina(10.0f);
        }
        else if (heightfall <= MediumHeightForLandingEffect)
        {
            // medium land
            GetCharacterStaminaComponent().RemoveStamina(20.0f);
        }
        else if (heightfall <= HighHeightForLandingEffect)
        {
            // high land
            GetCharacterStaminaComponent().RemoveStamina(50.0f);
        }
        else if (heightfall > HighHeightForLandingEffect)
        {
            // death land
            GetCharacterStaminaComponent().RemoveStamina(100.0f);
        }
    }

    private void ApplyNoHealthEffect()
    {
        // vytvorime (spawn) DeadCamBody do levelu na misto kde se aktualne nachazela kamera
        deadCamBody = 
            GD.Load<PackedScene>("res://player/character_systems/dead_cam_body.tscn").Instantiate() as dead_cam_body;
        GameMaster.GM.GetLevelLoader().GetActualLevelScene().AddChild(deadCamBody);

        // aktivujeme DeadCam
        deadCamBody.ActivateDeadCam();
        deadCamBody.ApplyCentralImpulse(GetLastMotion()*50f);
    }

    public CharacterHealthComponent GetCharacterHealthComponent() { return characterHealthComponent; }
    public CharacterStaminaComponent GetCharacterStaminaComponent() {  return characterStaminaComponent; }
    public CharacterInventoryComponent GetInventoryComponent() { return characterInventoryComponent; }
    public CharacterStairsComponent GetCharacterStairsComponent() { return characterStairsComponent; }
    public CharacterShootBallComponet GetCharacterShootBallComponet() { return characterShootBallComponet; }
    public CharacterUseLadderComponent GetCharacterUseLadderComponent() {  return characterUseLadderComponent; }
    public CharacterInfoHud GetCharacterInfoHud() { return characterInfoHud; }
    public Godot.Collections.Array<AudioStream> GetHurtAudios(){return HurtAudios;}
    public Godot.Collections.Array<AudioStream> GetDeathAudios() { return DeathAudios;}
    public Godot.Collections.Array<AudioStream> GetBodyFallAudios() { return BodyFallAudios; }

    public AudioStreamPlayer GetHurtPlayer() { return hurtPlayer; }

    public void PlaySpecificAudioOnPlayer(AudioStream newStream,float volumeDB = 0.0f,float pitchScale = 1.0f)
    {
        if(newStream != null && universalPlayer != null)
        {
            universalPlayer.Stream = newStream;
            universalPlayer.VolumeDb = volumeDB;
            universalPlayer.PitchScale = pitchScale;
            universalPlayer.Play();
        }
    }

    public override void FreeAll()
    {
        base.FreeAll();
        //deadCamBody.FreeAll();
    }
}
