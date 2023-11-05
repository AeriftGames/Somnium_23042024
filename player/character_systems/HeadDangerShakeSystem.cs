using Godot;
using System;

public partial class HeadDangerShakeSystem : Node
{
    [Export] public bool EnableShakeFromWorld = true;
    [Export] public float ShakeFade = 5.0f;
    public float ShakeStrenght = 0.0f;

    RandomNumberGenerator RnGenerator = new RandomNumberGenerator();

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
    public void ApplyUserParamShake(float newShakeStrenght,float newShakeFade)
    {
        ShakeStrenght += newShakeStrenght;
        ShakeFade = newShakeFade;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("testDangerShake"))
            ApplySmallInstantShake(0);

        if(ShakeStrenght > 0.0f && EnableShakeFromWorld)
        {
            ShakeStrenght = Mathf.Lerp(ShakeStrenght, 0, ShakeFade * (float)delta);

            Vector2 ShakeFinal = GetRandomOffset(ShakeStrenght)/50f;
            GD.Print("After Random: "+ShakeFinal);

            GameMaster.GM.GetFPSCharacter().GetFPSCharacterCamera().HOffset = ShakeFinal.X;
            GameMaster.GM.GetFPSCharacter().GetFPSCharacterCamera().VOffset = ShakeFinal.Y;
        }
    }

    public Vector2 GetRandomOffset(float newShakeStrenght)
    {
        RnGenerator.Randomize();
        return new Vector2(RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght),
            RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght));
    }
}
