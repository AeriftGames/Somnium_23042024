using Godot;
using System;

public partial class CCharacterStaminaComponent : CBaseComponent
{
    private FPSCharacterAction ourCharacterAction = null;

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

    private Godot.Timer timerStaminaRegenTimer = null;
    private Godot.Timer timerStaminaExhaustTimer = null;

    private bool StaminaExhaustActive = false;

    // GUI
    [ExportGroupAttribute("GUI SETTINGS")]
    [Export] public bool ShowStaminaGUIEnable = true;

    private Control StaminaScreenControl = null;
    private ProgressBar StaminaProgressBar = null;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);
        ourCharacterAction = ourCharacterBase as FPSCharacterAction;

        // GUI
        StaminaScreenControl = GetNode<Control>("%StaminaScreenControl");
        StaminaProgressBar = GetNode<ProgressBar>("%StaminaProgressBar");

        // RegenTickTimer init
        timerStaminaRegenTimer = new Godot.Timer();
        var callable_spawn = new Callable(this, "RegenTick");
        timerStaminaRegenTimer.Connect("timeout", callable_spawn);
        timerStaminaRegenTimer.WaitTime = 0.2;
        timerStaminaRegenTimer.OneShot = false;
        AddChild(timerStaminaRegenTimer);
        timerStaminaRegenTimer.Stop();

        // ExhaustTimer init
        timerStaminaExhaustTimer = new Godot.Timer();
        timerStaminaExhaustTimer.Connect("timeout", new Callable(this, "StaminaExhaustEnd"));
        timerStaminaExhaustTimer.OneShot = true;
        AddChild(timerStaminaExhaustTimer);
        timerStaminaExhaustTimer.Stop();

        //
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
        if (ourCharacterAction == null) return;

        ActualStamina += value;
        if (ActualStamina > ActualmaxStamina)
            ActualStamina = ActualmaxStamina;

        ChangeUpdate();
    }

    public void RemoveStamina(float value)
    {
        if (ourCharacterAction == null) return;

        ActualStamina -= value;
        if (ActualStamina < 0)
            ActualStamina = 0;

        ChangeUpdate();
    }

    public void RegenTick()
    {
        if (ourCharacterAction == null) return;

        ActualStamina += ActualStaminaRegenVal;

        if (ActualStamina > ActualmaxStamina)
            ActualStamina = ActualmaxStamina;

        ChangeUpdate();
    }

    public void RemoveStaminaJump() 
    { 
        RemoveStamina(2.0f); 
    }
    public void RemoveStaminaLand() 
    {
        RemoveStamina(3.0f);
        SetStaminaRegenVal(0.0f);
    }

    // STAMINA EXHAUST
    public void ApplyStaminaExhaustForTime(float newTimeForNotRecovery)
    {
        StaminaExhaustActive = true;
        ActualStamina = 0.0f;
        //SetStaminaRegenVal(0.0f);

        timerStaminaExhaustTimer.WaitTime = newTimeForNotRecovery;
        timerStaminaExhaustTimer.Start();
    }

    public bool GetStaminaExhaustActive() { return StaminaExhaustActive; }

    public void StaminaExhaustEnd() 
    {
        ActualStamina = 0.12f;
        timerStaminaExhaustTimer.Stop();
        StaminaExhaustActive = false;
    }

    // GUI

    private void ChangeUpdate()
    {
        if (ourCharacterAction == null) return;
        if (StaminaProgressBar == null) return;

        // Set gui visible ?
        StaminaScreenControl.Visible = ShowStaminaGUIEnable;

        // apply gui
        StaminaProgressBar.Value = ActualStamina;
    }

    // STATES LOGIC
    public override void _Process(double delta)
    {
        base._Process(delta);

        // pokud jsme uplne vycerpani - musime pockat
        if (StaminaExhaustActive) return;

        // Idle State
        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "IdlePlayerState" &&
            ActiveFastRegenForStanding)
        {
            SetStaminaRegenVal(0.3f);
            SetStaminaRegenTick(0.05f);
        }

        else
        // Crouching move State
        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "CrouchMovePlayerState" &&
            ActiveFastRegenForCrouching)
        {
            SetStaminaRegenVal(0.25f);
            SetStaminaRegenTick(0.05f);
        }

        else
        // Crouching idle State
        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "IdleCrouchPlayerState" &&
            ActiveFastRegenForCrouching)
        {
            SetStaminaRegenVal(0.35f);
            SetStaminaRegenTick(0.05f);
        }

        else
        // Walking State
        if (ourCharacterBase.GetCharacterMovementComponent().GetIsWantSprint() == false &&
            ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() < 2.6f)
        {
            SetStaminaRegenVal(0.18f);
        }

        else
        // Running State
        if (ourCharacterBase.GetCharacterMovementComponent().GetIsWantSprint() &&
            ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 2.6f &&
            ActiveStaminaForSprint)
        {
            RemoveStamina(0.05f);
            SetStaminaRegenVal(0.0f);
        }
    }
}
