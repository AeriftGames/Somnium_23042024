using Godot;
using System;

public partial class FPSCharacterAction : FPSCharacterMoveAnim
{
    private CCharacterFlashlightComponent FlashlightComponent = null;
    private CCharacterUseActionComponent UseActionComponent = null;
    
    private CCharacterStaminaComponent StaminaComponent = null;
    private CCharacterHealthComponent HealthComponent = null;

    public CCharacterFlashlightComponent GetFlashlightComponent() { return FlashlightComponent; }
    public CCharacterUseActionComponent GetUseActionComponent() { return UseActionComponent; }
    public CCharacterStaminaComponent GetStaminaComponent() { return StaminaComponent; }
    public CCharacterHealthComponent GetHealthComponent() { return HealthComponent; }

    public override void _Ready()
    {
        base._Ready();

        FlashlightComponent = GetBaseComponents().GetNode<CCharacterFlashlightComponent>("BaseFlashlightComponent");
        FlashlightComponent.PostInit(this);

        UseActionComponent = GetBaseComponents().GetNode<CCharacterUseActionComponent>("BaseUseActionComponent");
        UseActionComponent.PostInit(this);

        StaminaComponent = GetBaseComponents().GetNode<CCharacterStaminaComponent>("BaseStaminaComponent");
        StaminaComponent.PostInit(this);

        HealthComponent = GetBaseComponents().GetNode<CCharacterHealthComponent>("BaseHealthComponent");
        HealthComponent.PostInit(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GetUseActionComponent().Update(delta);
    }
}
