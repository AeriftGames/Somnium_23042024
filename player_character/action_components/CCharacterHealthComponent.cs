using Godot;
using System;

public partial class CCharacterHealthComponent : CBaseComponent
{
    private FPSCharacterAction ourCharacterAction = null;

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

    private Godot.Timer timerHealthRegenTimer = null;

    private bool isAlive = true;

    // GUI
    [ExportGroupAttribute("GUI SETTINGS")]
    [Export] public bool ShowHealthGUIEnable = true;

    private Control HealthScreenControl = null;
    private ProgressBar HealthProgressBar = null;


    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);
        ourCharacterAction = ourCharacterBase as FPSCharacterAction;

        // GUI
        HealthScreenControl = GetNode<Control>("%HealthScreenControl");
        HealthProgressBar = GetNode<ProgressBar>("%HealthProgressBar");

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
        if (ourCharacterAction == null) return;
        if (HealthProgressBar == null) return;

        // DEAD
        if (ActualHealth < 1.0f && isAlive)
        {
            GD.Print("you are dead test");
            isAlive = false;
        }

        // Set gui visible ?
        HealthScreenControl.Visible = ShowHealthGUIEnable;

        // apply gui
        HealthProgressBar.Value = ActualHealth;
    }

    // STATES LOGIC
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("test_damage"))
            RemoveHealth(10);

        if (Input.IsActionJustPressed("test_regen"))
            AddHealth(10);
    }
}
