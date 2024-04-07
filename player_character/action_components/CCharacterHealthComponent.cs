using Godot;
using System;

public partial class CCharacterHealthComponent : CBaseComponent
{
    private FPSCharacterAction ourCharacterAction = null;
    private HealthMathComponent healthMathComponent = null;
    private DamageHud damageHudComponent = null;
    private CHealthAudioComponent healthAudioComponent = null;

    // GUI
    [ExportGroupAttribute("GUI SETTINGS")]
    [Export] public bool ShowHealthGUIEnable = true;

    [ExportGroupAttribute("DAMAGE SHAKE SETTINGS")]
    [Export] public float DamageStrengthShake = 5.0f;
    [Export] public float DamageFallOffShake = 10.0f;

    private Control HealthScreenControl = null;
    private ProgressBar HealthProgressBar = null;


    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);
        ourCharacterAction = ourCharacterBase as FPSCharacterAction;

        healthMathComponent = GetNode<HealthMathComponent>("%HealthMathComponent");
        healthMathComponent.PostInit(ourCharacterBase);

        damageHudComponent = GetNode<DamageHud>("%DamageScreenControl");
        damageHudComponent.PostInit();

        healthAudioComponent = GetNode<CHealthAudioComponent>("%HealthAudioComponent");
        healthAudioComponent.PostInit(ourCharacterAction);
    }

    public HealthMathComponent GetHealthMath() { return healthMathComponent; }
    public DamageHud GetDamageHud() { return damageHudComponent; }
    public CHealthAudioComponent GetHealthAudio() { return healthAudioComponent; }

    // STATES LOGIC
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("test_damage"))
            ApplyDamage(10.0f);

        if (Input.IsActionJustPressed("test_regen"))
            GetHealthMath().AddHealth(10);
    }

    public void ApplyDamage(float newDamage)
    {
        GetHealthMath().RemoveHealth(newDamage);
        GetDamageHud().ApplyCentralDamageEffect(newDamage);
        GetHealthAudio().PlayHurtAudio(newDamage);

        ourCharacterAction.GetCharacterCameraShakeComponent().
            ApplyUserParamShake(DamageStrengthShake, DamageFallOffShake);
    }

    public void ApplyHeal(float newHeal)
    {
        GetHealthMath().AddHealth(newHeal); 
    }
}
