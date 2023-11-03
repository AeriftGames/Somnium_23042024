using Godot;
using System;

public partial class HeadDangerShakeSystem : Node
{
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
        ShakeStrenght = newShakeStrenght;
        ShakeFade = newShakeFade;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("testDangerShake"))
            ApplySmallInstantShake(0);

        if(ShakeStrenght > 0.0f)
        {
            ShakeStrenght = Mathf.Lerp(ShakeStrenght, 0, ShakeFade * (float)delta);

            GameMaster.GM.GetFPSCharacter().GetFPSCharacterCamera().HOffset = GetRandomOffset().X;
            GameMaster.GM.GetFPSCharacter().GetFPSCharacterCamera().VOffset = GetRandomOffset().Y;
        }
    }

    public Vector2 GetRandomOffset()
    {
        return new Vector2(RnGenerator.RandfRange(-ShakeStrenght, ShakeStrenght),
            RnGenerator.RandfRange(-ShakeStrenght,ShakeStrenght));
    }
}
