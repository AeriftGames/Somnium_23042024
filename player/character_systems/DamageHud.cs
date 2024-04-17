using Godot;
using System;

public partial class DamageHud : Control
{
    ShaderMaterial damageShader = null;
    AnimationPlayer animationPlayer = null;
    TextureRect bloodTextureRect = null;

    Control BloodScreenHealth = null;
    CDamageDirIndicator DamageDirIndicator = null;

    [Export] float speedEffect = 1.0f;
    [Export] float defaultVal = 0.8f;
    [Export] float minMultitiplier = 0.6f;
    [Export] float maxMutliplier = 0.3f;

    float actualVal = 0.8f;

    public void PostInit()
    {
        damageShader = GetNode<ColorRect>("ColorRect_Vignette").Material as ShaderMaterial;
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        bloodTextureRect = GetNode<TextureRect>("TextureRect_blood");

        BloodScreenHealth = GetNode<Control>("BloodScreenHealth");

        DamageDirIndicator = GetNode<CDamageDirIndicator>("DamageDirIndicator");
        DamageDirIndicator.PostInit();
    }

    public void ApplyCentralDamageEffect(float newIntensity)
    {
        actualVal = GetEffectMultiplierFromIntensity(Mathf.Clamp(newIntensity, 0.0f, 1.0f));
    }

    public void StartBloodDeathHud()
    {
        animationPlayer.Play("start_blood_death_hud");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // update damageShader
        if(damageShader == null) return;

        actualVal = Mathf.Lerp(actualVal,defaultVal,speedEffect*(float)delta);
        damageShader.SetShaderParameter("multiplier", actualVal);
    }

    private float GetEffectMultiplierFromIntensity(float newIntensity)
    {
        return minMultitiplier - (newIntensity * (minMultitiplier-maxMutliplier));
    }

    public void SetInitBloodScreen()
    {
        bloodTextureRect.PivotOffset = bloodTextureRect.Size / 2;
    }

    public void UpdateBloodScreenHealth()
    {
        FPSCharacterAction a = CGameMaster.GM.GetGame().GetFPSCharacterBase() as FPSCharacterAction;
        if (a == null) return;

        float actual = a.GetHealthComponent().GetHealthMath().ActualHealth;
        float max = a.GetHealthComponent().GetHealthMath().ActualMaxHealth;

        float aa = 255.0f - (255.0f / (max / actual));
        byte v = (byte)aa;
        BloodScreenHealth.Modulate = Color.Color8(255, 255, 255, v);
        GD.Print(aa);
    }
}
