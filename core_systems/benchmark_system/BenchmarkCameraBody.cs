using Godot;
using System;
using System.Dynamic;
using System.Threading.Tasks;

public partial class BenchmarkCameraBody : CharacterBody3D
{
    Camera3D benchmarkCamera;

    [Export] public bool EnableShakeFromWorld = true;
    [Export] public float ShakeFade = 5.0f;
    public float ShakeStrenght = 0.0f;

    RandomNumberGenerator RnGenerator = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        benchmarkCamera = GetNode<Camera3D>("BenchmarkCamera");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (ShakeStrenght > 0.0f && EnableShakeFromWorld)
        {
            ShakeStrenght = Mathf.Lerp(ShakeStrenght, 0, ShakeFade * (float)delta);

            Vector2 ShakeFinal = GetRandomOffset(ShakeStrenght) / 50f;
            //GD.Print("After Random: " + ShakeFinal);

            benchmarkCamera.HOffset = ShakeFinal.X;
            benchmarkCamera.VOffset = ShakeFinal.Y;
        }
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

    public Vector2 GetRandomOffset(float newShakeStrenght)
    {
        RnGenerator.Randomize();
        return new Vector2(RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght),
            RnGenerator.RandfRange(-newShakeStrenght, newShakeStrenght));
    }

    public Camera3D GetCamera() { return benchmarkCamera; }
}
