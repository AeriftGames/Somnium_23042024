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
        ourCharacterBase.GetCharacterFovComponent().SetFovOffset("Breath", BreathFovOffset);
    }

    public void PauseBreathing() { GetNode<AnimationPlayer>("AnimationPlayer_Breathing").Pause(); }
    public void PlayBreathing() { GetNode<AnimationPlayer>("AnimationPlayer_Breathing").Play(); }
}
