using Godot;
using System;

public partial class CCharacterHealthComponent : CBaseComponent
{
    FPSCharacterAction ourActionCharacter = null;

    [Export] public float StartActualHealth = 100.0f;
    [Export] public float StartMaxHealth = 100.0f;
    [Export] public float StartHealthRegenVal = 1.0f;
    [Export] public float StartHealthRegenTick = 0.5f;
    [Export] public bool StartHealthRegenEnable = false;

    [Export] public float ActualHealth = 100;
    [Export] public float ActualMaxHealth = 100;
    [Export] public float ActualHealthRegenVal = 0.1f;
    [Export] public float ActualHealthRegenTick = 0.5f;
    [Export] public bool ActualHealthRegenEnable = false;

    Godot.Timer timerHealthRegenTimer = null;

    private bool isAlive = true;

    DamageHud damageHud = null;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        ourActionCharacter = newCharacterBase as FPSCharacterAction;

        damageHud = ourActionCharacter.GetNodeOrNull<DamageHud>("AllHuds/DamageHud");

        //RegenTickTimer init
        timerHealthRegenTimer = new Godot.Timer();
        var callable_spawn = new Callable(this, "RegenTick");
        timerHealthRegenTimer.Connect("timeout", callable_spawn);
        timerHealthRegenTimer.WaitTime = 0.2;
        timerHealthRegenTimer.OneShot = false;
        AddChild(timerHealthRegenTimer);
        timerHealthRegenTimer.Stop();

        // First init data
        SetAllData(StartActualHealth, StartMaxHealth, StartHealthRegenVal,
            StartHealthRegenTick, StartHealthRegenEnable);
    }

    public float GetHealth() { return ActualHealth; }
    public float GetMaxHealth() { return ActualMaxHealth; }
    public float GetHealthRegenVal() { return ActualHealthRegenVal; }
    public float GetHealthRegenTick() { return ActualHealthRegenTick; }
    public bool GetHealthRegenEnable() { return ActualHealthRegenEnable; }
    public bool GetAlive() { return isAlive; }
    public void SetHealth(float value) { ActualHealth = value; ChangeUpdate(); }
    public void SetMaxHealth(float value) { ActualMaxHealth = value; ChangeUpdate(); }
    public void SetHealthRegenVal(float value) { ActualHealthRegenVal = value; }
    public void SetHealthRegenTick(float value) { ActualHealthRegenTick = value; timerHealthRegenTimer.WaitTime = value; }
    public void SetHealthRegenEnable(bool value)
    {
        ActualHealthRegenEnable = value;
        if (value)
            timerHealthRegenTimer.Start();
        else
            timerHealthRegenTimer.Stop();
    }

    public void SetAllData(float newActualHealth, float newMaxHealth, float newHealthRegenVal, float newHealthRegenTick,
        bool newHealthRegenEnable)
    {
        SetHealth(newActualHealth);
        SetMaxHealth(newMaxHealth);
        SetHealthRegenVal(newHealthRegenVal);
        SetHealthRegenTick(newHealthRegenTick);
        SetHealthRegenEnable(newHealthRegenEnable);
    }

    public void AddHealth(float value)
    {
        if (!isAlive) return;

        ActualHealth += value;
        if (ActualHealth > ActualMaxHealth)
            ActualHealth = ActualMaxHealth;

        ChangeUpdate();
    }

    public void RemoveHealth(float value)
    {
        if (!isAlive) return;

        ActualHealth -= value;
        if (ActualHealth < 0)
            ActualHealth = 0;

        // Effects

        // pokud existuje damageHud - aplikujeme damage effect
        if (damageHud != null)
            damageHud.ApplyCentralDamageEffect(GetDamageIntensityFromDamageValue(value));
        /*
        // shake
        ownCharacter.GetObjectCamera().ShakeCameraTest(GetDamageIntensityFromDamageValue(value), 0.2f, 5.0f, 4.0f);

        // audio
        UniversalFunctions.PlayRandomSound(ownCharacter.GetHurtPlayer(), ownCharacter.GetHurtAudios(), 0, 1);
        */
        //
        ChangeUpdate();
    }

    public void RegenTick()
    {
        if (!isAlive) return;

        ActualHealth += ActualHealthRegenVal;

        if (ActualHealth > ActualMaxHealth)
            ActualHealth = ActualMaxHealth;

        ChangeUpdate();
    }

    private void ChangeUpdate()
    {
        /*
        if (ourActionCharacter == null) return;
        if (ownCharacter.GetCharacterInfoHud() == null) return;
        */
        // DEAD
        if (ActualHealth < 1.0f && isAlive)
        {
            GD.Print("you are dead test");
            isAlive = false;
            /*
            // audio
            UniversalFunctions.PlayRandomSound(ownCharacter.GetHurtPlayer(), ownCharacter.GetDeathAudios(), 1.0f, 0.75f);
            
            // event dead
            ownCharacter.EventDead(FPSCharacter_BasicMoving.ECharacterReasonDead.NoHealth);*/
        }
        /*
        ownCharacter.GetCharacterInfoHud().SetHealthDataFromPlayer();*/
    }

    public float GetDamageIntensityFromDamageValue(float value)
    {
        if (value > 49)
            return 1.0f;
        else if (value > 19)
            return 0.6f;
        else
            return 0.15f;
    }

    public DamageHud GetDamageHud()
    {
        return damageHud;
    }
}
