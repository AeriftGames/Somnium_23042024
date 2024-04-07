using Godot;
using System;

public partial class CCharacterCameraShakeComponent : CBaseComponent
{
    [Export] public bool EnableShakeFromWorld = true;
    [Export] public float ShakeFade = 5.0f;
    public float ShakeStrenght = 0.0f;

    RandomNumberGenerator RnGenerator = new RandomNumberGenerator();

    //
    Node3D ShakeNode = null;
    ShakeLerp camShakeLerp = null;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        // init camShakeLerp
        ShakeNode = ourCharacterBase.GetCharacterLookComponent().GetCameraShakeRot();
        camShakeLerp = new ShakeLerp();
        camShakeLerp.Init(ShakeNode);
    }

    public void ApplySmallInstantShake(float newShakeStrenght)
    {
        ShakeStrenght = 1f;
        ShakeFade = 5.0f;
    }

    public void ApplyMediumStrengthInstanShake(float newShakeStrenght)
    {
        ShakeStrenght = 2f;
        ShakeFade = 8.5f;
    }
    public void ApplyUserParamShake(float newShakeStrenght, float newShakeFade)
    {
        ShakeStrenght += newShakeStrenght;
        ShakeFade = newShakeFade;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (ShakeStrenght > 0.0f && EnableShakeFromWorld)
        {
            ShakeStrenght = Mathf.Lerp(ShakeStrenght, 0, ShakeFade * (float)delta);

            Vector2 ShakeFinal = GetRandomOffset(ShakeStrenght) / 50f;
            //GD.Print("After Random: "+ShakeFinal);

            ourCharacterBase.GetCharacterLookComponent().GetMainCamera().HOffset = ShakeFinal.X;
            ourCharacterBase.GetCharacterLookComponent().GetMainCamera().VOffset = ShakeFinal.Y;
        }

        //

        // camShakeLerp logic update
        camShakeLerp.Update(delta);
    }

    public Vector2 GetRandomOffset(float newShakeStrenght)
    {
        RnGenerator.Randomize();
        return new Vector2(RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght),
            RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght));
    }

    // funkce pro camera shake rot

    public void ShakeCameraTest(float newIntensity, float newTime, float newShakeSpeedTo, float newShakeSpeedBack)
    {
        if (camShakeLerp != null)
            camShakeLerp.StartBasicShake(newIntensity, newTime, newShakeSpeedTo, newShakeSpeedBack);
    }

    public void ShakeCameraRotation(float newIntensity, float newTime, float newShakeSpeedTo, float newShakeSpeedBack,
        bool newApplyRotX = true, bool newApplyRotY = true, bool newApplyRotZ = true)
    {
        if (camShakeLerp != null)
            camShakeLerp.StartBasicShake(newIntensity, newTime, newShakeSpeedTo, newShakeSpeedBack,
                newApplyRotX, newApplyRotY, newApplyRotZ);
    }
}
