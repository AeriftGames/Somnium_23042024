using Godot;
using System;
using System.Dynamic;
using System.Threading.Tasks;

public partial class BenchmarkCameraBody : CharacterBody3D
{
    Camera3D benchmarkCamera;
    Label FPSLabel;
    Label QualityLabel;

    InBenchmarkMenu inBenchmarkMenu;

    [Export] public bool EnableShakeFromWorld = true;
    [Export] public float ShakeFade = 5.0f;
    public float ShakeStrenght = 0.0f;

    RandomNumberGenerator RnGenerator = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        benchmarkCamera = GetNode<Camera3D>("BenchmarkCamera");
        FPSLabel = GetNode<Label>("BenchmarkCamera/Control/VBoxContainer/HBoxContainer/FPSLabel");
        QualityLabel = GetNode<Label>("BenchmarkCamera/Control/VBoxContainer/HBoxContainer3/QualityText");
        inBenchmarkMenu = GetNode<InBenchmarkMenu>("BenchmarkCamera/Control/InBenchmarkMenu");

        PostInit();
    }

    public InBenchmarkMenu GetInBenchmarkMenu() {  return inBenchmarkMenu; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        FPSLabel.Text = Engine.GetFramesPerSecond().ToString();

        if (Input.IsActionJustPressed("EscapeAction"))
            inBenchmarkMenu.SetActive(!inBenchmarkMenu.GetActive());

        if (ShakeStrenght > 0.0f && EnableShakeFromWorld)
        {
            ShakeStrenght = Mathf.Lerp(ShakeStrenght, 0, ShakeFade * (float)delta);

            Vector2 ShakeFinal = GetRandomOffset(ShakeStrenght) / 50f;
            //GD.Print("After Random: " + ShakeFinal);

            benchmarkCamera.HOffset = ShakeFinal.X;
            benchmarkCamera.VOffset = ShakeFinal.Y;
        }
    }

    public async void PostInit()
    {
        await ToSignal(GameMaster.GM.GetLevelLoader(), CLevelLoader.SignalName.LevelLoadComplete);

        await Task.Delay(1000);

        // vyber textu aktualni kvality
        switch (GameMaster.GM.GetBenchmarkSystem().GetActualQualityBenchmark())
        {
            case 0: QualityLabel.Text = "lowest quality"; break;
            case 1: QualityLabel.Text = "low quality"; break;
            case 2: QualityLabel.Text = "medium quality"; break;
            case 3: QualityLabel.Text = "high quality"; break;
            case 4: QualityLabel.Text = "highest quality"; break;
        }

        GameMaster.GM.GetSettings().Apply_UnlockMaxFps(true, true);
        GameMaster.GM.GetSettings().Apply_DisableVsync(true, true);
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
