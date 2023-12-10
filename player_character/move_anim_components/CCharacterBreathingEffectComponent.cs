using Godot;
using System;

public partial class CCharacterBreathingEffectComponent : CBaseComponent
{
    // nesahat - je to ovladane animaci
    [Export] public float BreathFovOffset = 0.0f;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);
    }

    public void Update(double delta)
    {
        ourCharacterBase.GetCharacterLookComponent().SetFovOffset("Breath", BreathFovOffset);
    }
}
