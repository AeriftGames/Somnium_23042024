using Godot;
using System;

public partial class HealthSystem : Godot.GodotObject
{
    FPSCharacter_Inventory ownnCharacter = null;

    private float actualHealth = 100;
    private float maxHealth = 100;
    private float healthRegenVal = 0.1f;
    private float healthRegenTick = 0.5f;
    private bool healthRegenEnable = false;

    Godot.Timer timerHealthRegenTimer = null;

    private bool isAlive = true;

    DamageHud damageHud = null;

    public HealthSystem(FPSCharacter_Inventory ownerInstance) 
    {
        ownnCharacter = ownerInstance;

        damageHud = ownnCharacter.GetNodeOrNull<DamageHud>("AllHuds/DamageHud");

        //RegenTickTimer init
        timerHealthRegenTimer = new Godot.Timer();
        var callable_spawn = new Callable(this, "RegenTick");
        timerHealthRegenTimer.Connect("timeout", callable_spawn);
        timerHealthRegenTimer.WaitTime = 0.2;
        timerHealthRegenTimer.OneShot = false;
        ownnCharacter.AddChild(timerHealthRegenTimer);
        timerHealthRegenTimer.Stop();
    }

    public float GetHealth() { return actualHealth; }
    public float GetMaxHealth() { return maxHealth; }
    public float GetHealthRegenVal() { return healthRegenVal; }
    public float GetHealthRegenTick() { return healthRegenTick; }
    public bool GetHealthRegenEnable() { return healthRegenEnable; }
    public bool GetAlive() { return isAlive; }
    public void SetHealth(float value) { actualHealth = value; ChangeUpdate(); }
    public void SetMaxHealth(float value) { maxHealth = value; ChangeUpdate(); }
    public void SetHealthRegenVal(float value) { healthRegenVal = value; }
    public void SetHealthRegenTick(float value) { healthRegenTick = value; timerHealthRegenTimer.WaitTime = value; }
    public void SetHealthRegenEnable(bool value) 
    { 
        healthRegenEnable = value; 
        if(value)
            timerHealthRegenTimer.Start(); 
        else
            timerHealthRegenTimer.Stop();
    }

    public void SetAllData(float newActualHealth,float newMaxHealth,float newHealthRegenVal,float newHealthRegenTick,
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

        actualHealth += value;
        if(actualHealth > maxHealth)
            actualHealth = maxHealth;

        ChangeUpdate();
    }

    public void RemoveHealth(float value) 
    {
        if (!isAlive) return;

        actualHealth -= value;
        if(actualHealth < 0)
            actualHealth = 0;

        ChangeUpdate();

        // Effects

        // pokud existuje damageHud - aplikujeme damage effect
        if (damageHud != null)
            damageHud.ApplyCentralDamageEffect(GetDamageIntensityFromDamageValue(value));

        // shake
        ownnCharacter.GetObjectCamera().ShakeCameraTest(GetDamageIntensityFromDamageValue(value),0.2f,5.0f,4.0f);

        // audio
        UniversalFunctions.PlayRandomSound(ownnCharacter.GetHurtPlayer(), ownnCharacter.GetHurtAudios(), 0, 1);
    }

    public void RegenTick()
    {
        if (!isAlive) return;

        actualHealth += healthRegenVal;

        if(actualHealth > maxHealth)
            actualHealth = maxHealth;

        ChangeUpdate();
    }

    private void ChangeUpdate()
    {
        if (ownnCharacter == null) return;
        if (ownnCharacter.GetCharacterInfoHud() == null) return;

        // DEAD
        if (actualHealth < 1.0f && isAlive)
        {
            isAlive = false;
            ownnCharacter.EventDead(FPSCharacter_BasicMoving.ECharacterReasonDead.NoHealth);
        }

        ownnCharacter.GetCharacterInfoHud().SetDataFromPlayer();
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
}
