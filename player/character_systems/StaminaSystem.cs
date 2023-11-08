using Godot;
using System;

public partial class StaminaSystem : Node
{
	FPSCharacter_Inventory ownCharacter = null;

    [Export] public float initStamina = 50.0f;
    [Export] public float initMaxStamina = 100.0f;
    [Export] public float initStaminaRegenVal = 0.1f;
    [Export] public float initStaminaRegenTick = 0.5f;
    [Export] public bool initStaminaRegenEnable = false;

    [Export] public bool activeStaminaForJump = true;
    [Export] public bool activeStaminaForLand = true;
    [Export] public bool activeStaminaForSprint = true;
    [Export] public bool activeFastRegenForStanding = true;
    [Export] public bool activeFastRegenForCrouching = true;

    private float actualStamina = 100;
    private float maxStamina = 100;
    private float staminaRegenVal = 0.1f;
    private float staminaRegenTick = 0.5f;
    private bool staminaRegenEnable = false;

    Godot.Timer timerStaminaRegenTimer = null;

    public void StartInit(FPSCharacter_Inventory ownerInstance)
	{
		ownCharacter = ownerInstance;

        //RegenTickTimer init
        timerStaminaRegenTimer = new Godot.Timer();
        var callable_spawn = new Callable(this, "RegenTick");
        timerStaminaRegenTimer.Connect("timeout", callable_spawn);
        timerStaminaRegenTimer.WaitTime = 0.2;
        timerStaminaRegenTimer.OneShot = false;
        AddChild(timerStaminaRegenTimer);
        timerStaminaRegenTimer.Stop();

        SetAllData(initStamina, initMaxStamina, initStaminaRegenVal, initStaminaRegenTick, initStaminaRegenEnable);
    }

    public float GetStamina() { return actualStamina; }
    public float GetMaxStamina() { return maxStamina; }
    public float GetStaminaRegenVal() { return staminaRegenVal; }
    public float GetStaminaRegenTick() { return staminaRegenTick; }
    public bool GetStaminaRegenEnable() { return staminaRegenEnable; }
    public void SetStamina(float value) { actualStamina = value; ChangeUpdate(); }
    public void SetMaxStamina(float value) { maxStamina = value; ChangeUpdate(); }
    public void SetStaminaRegenVal(float value) { staminaRegenVal = value; }
    public void SetStaminaRegenTick(float value) { staminaRegenTick = value; timerStaminaRegenTimer.WaitTime = value; }
    public void SetStaminaRegenEnable(bool value)
    {
        staminaRegenEnable = value;
        if (value)
            timerStaminaRegenTimer.Start();
        else
            timerStaminaRegenTimer.Stop();
    }
    public void SetAllData(float newActualStamina, float newMaxStamina, float newStaminaRegenVal, float newStaminaRegenTick,
        bool newStaminaRegenEnable)
    {
        SetStamina(newActualStamina);
        SetMaxStamina(newMaxStamina);
        SetStaminaRegenVal(newStaminaRegenVal);
        SetStaminaRegenTick(newStaminaRegenTick);
        SetStaminaRegenEnable(newStaminaRegenEnable);
    }

    public void AddStamina(float value)
    {
        if (!ownCharacter.GetHealthSystem().GetAlive()) return;

        actualStamina += value;
        if(actualStamina > maxStamina)
            actualStamina = maxStamina;

        ChangeUpdate();
    }

    public void RemoveStamina(float value)
    {
        if (!ownCharacter.GetHealthSystem().GetAlive()) return;

        actualStamina -= value;
        if (actualStamina < 0)
            actualStamina = 0;

        ChangeUpdate();
    }

    public void RegenTick()
    {
        if (!ownCharacter.GetHealthSystem().GetAlive()) return;

        actualStamina += staminaRegenVal;

        if (actualStamina > maxStamina)
            actualStamina = maxStamina;

        ChangeUpdate();
    }

    private void ChangeUpdate()
    {
        if (ownCharacter == null) return;
        if (ownCharacter.GetCharacterInfoHud() == null) return;

        ownCharacter.GetCharacterInfoHud().SetStaminaDataFromPlayer();
    }
}
