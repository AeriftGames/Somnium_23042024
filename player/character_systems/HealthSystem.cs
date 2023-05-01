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

    public HealthSystem(FPSCharacter_Inventory ownerInstance) 
    {
        ownnCharacter = ownerInstance;

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
        actualHealth += value;
        if(actualHealth > maxHealth)
            actualHealth = maxHealth;

        ChangeUpdate();
    }

    public void RemoveHealth(float value) 
    {
        actualHealth -= value;
        if(actualHealth < 0)
            actualHealth = 0;

        ChangeUpdate();
    }

    public void RegenTick()
    {
        actualHealth += healthRegenVal;

        if(actualHealth > maxHealth)
            actualHealth = maxHealth;

        ChangeUpdate();
    }

    private void ChangeUpdate()
    {
        if (ownnCharacter == null) return;
        if (ownnCharacter.GetCharacterInfoHud() == null) return;

        if (actualHealth < 1.0f)
            ownnCharacter.EventDead(FPSCharacter_BasicMoving.ECharacterReasonDead.NoHealth);

        ownnCharacter.GetCharacterInfoHud().SetDataFromPlayer();
    }
}
