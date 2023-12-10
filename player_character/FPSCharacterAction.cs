using Godot;
using System;

public partial class FPSCharacterAction : FPSCharacterMoveAnim
{
    private CCharacterFlashlightComponent FlashlightComponent = null;
    public CCharacterFlashlightComponent GetFlashlightComponent() { return FlashlightComponent; }

    public override void _Ready()
    {
        base._Ready();

        FlashlightComponent = GetBaseComponents().GetNode<CCharacterFlashlightComponent>("BaseFlashlightComponent");
        FlashlightComponent.PostInit(this);
    }
}
