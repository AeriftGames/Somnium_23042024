using Godot;
using System;

public partial class CCharacterCameraShakeComponent : CBaseComponent
{
    [Export] public bool EnableShakeFromWorld = true;
    [Export] public float ShakeFade = 5.0f;
    public float ShakeStrenght = 0.0f;

    RandomNumberGenerator RnGenerator = new RandomNumberGenerator();

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);
    }

    public void ApplySmallInstantShake(float newShakeStrenght)
    {
        ShakeStrenght = 0.1f;
        ShakeFade = 5.0f;
    }

    public void ApplyMediumLongShake(float newShakeStrenght)
    {
        ShakeStrenght = 0.02f;
        ShakeFade = 0.5f;
    }
    public void ApplyUserParamShake(float newShakeStrenght, float newShakeFade)
    {
        ShakeStrenght += newShakeStrenght;
        ShakeFade = newShakeFade;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("testDangerShake"))
            ApplySmallInstantShake(0);

        if (ShakeStrenght > 0.0f && EnableShakeFromWorld)
        {
            ShakeStrenght = Mathf.Lerp(ShakeStrenght, 0, ShakeFade * (float)delta);

            Vector2 ShakeFinal = GetRandomOffset(ShakeStrenght) / 50f;
            //GD.Print("After Random: "+ShakeFinal);

            ourCharacterBase.GetCharacterLookComponent().GetMainCamera().HOffset = ShakeFinal.X;
            ourCharacterBase.GetCharacterLookComponent().GetMainCamera().VOffset = ShakeFinal.Y;
        }
    }

    public Vector2 GetRandomOffset(float newShakeStrenght)
    {
        RnGenerator.Randomize();
        return new Vector2(RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght),
            RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght));
    }
}
