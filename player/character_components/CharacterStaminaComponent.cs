using Godot;
using System;

public partial class CharacterStaminaComponent : Node
{
	FPSCharacter_Inventory ownCharacter = null;

    [Export] public float InitStamina = 50.0f;
    [Export] public float InitMaxStamina = 100.0f;
    [Export] public float InitStaminaRegenVal = 0.1f;
    [Export] public float InitStaminaRegenTick = 0.5f;
    [Export] public bool InitStaminaRegenEnable = true;

    [Export] public bool ActiveStaminaForJump = true;
    [Export] public bool ActiveStaminaForLand = true;
    [Export] public bool ActiveStaminaForSprint = true;
    [Export] public bool ActiveFastRegenForStanding = true;
    [Export] public bool ActiveFastRegenForCrouching = true;

    [Export] public float ActualStamina = 100;
    [Export] public float ActualmaxStamina = 100;
    [Export] public float ActualStaminaRegenVal = 0.1f;
    [Export] public float ActualStaminaRegenTick = 0.5f;
    [Export] public bool ActualStaminaRegenEnable = false;

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

        SetAllData(InitStamina, InitMaxStamina, InitStaminaRegenVal, InitStaminaRegenTick, InitStaminaRegenEnable);
    }

    public float GetStamina() { return ActualStamina; }
    public float GetMaxStamina() { return ActualmaxStamina; }
    public float GetStaminaRegenVal() { return ActualStaminaRegenVal; }
    public float GetStaminaRegenTick() { return ActualStaminaRegenTick; }
    public bool GetStaminaRegenEnable() { return ActualStaminaRegenEnable; }
    public void SetStamina(float value) { ActualStamina = value; ChangeUpdate(); }
    public void SetMaxStamina(float value) { ActualmaxStamina = value; ChangeUpdate(); }
    public void SetStaminaRegenVal(float value) { ActualStaminaRegenVal = value; }
    public void SetStaminaRegenTick(float value) { ActualStaminaRegenTick = value; timerStaminaRegenTimer.WaitTime = value; }
    public void SetStaminaRegenEnable(bool value)
    {
        ActualStaminaRegenEnable = value;
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
        if (!ownCharacter.GetCharacterHealthComponent().GetAlive()) return;

        ActualStamina += value;
        if(ActualStamina > ActualmaxStamina)
            ActualStamina = ActualmaxStamina;

        ChangeUpdate();
    }

    public void RemoveStamina(float value)
    {
        if (!ownCharacter.GetCharacterHealthComponent().GetAlive()) return;

        ActualStamina -= value;
        if (ActualStamina < 0)
            ActualStamina = 0;

        ChangeUpdate();
    }

    public void RegenTick()
    {
        if (!ownCharacter.GetCharacterHealthComponent().GetAlive()) return;

        ActualStamina += ActualStaminaRegenVal;

        if (ActualStamina > ActualmaxStamina)
            ActualStamina = ActualmaxStamina;

        ChangeUpdate();
    }

    private void ChangeUpdate()
    {
        if (ownCharacter == null) return;
        if (ownCharacter.GetCharacterInfoHud() == null) return;

        ownCharacter.GetCharacterInfoHud().SetStaminaDataFromPlayer();
    }
}
