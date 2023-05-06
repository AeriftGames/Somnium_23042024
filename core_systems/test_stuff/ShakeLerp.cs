using Godot;
using System;

public partial class ShakeLerp : Node
{
	bool isActive = false;
    Node3D applyNode = null;

	// for shake
	Godot.Timer shakeTimer = null;
    private Vector3 actualShakeRot = Vector3.Zero;
    private Vector3 needShakeRot = Vector3.Zero;    // zero = no shake
    private float shakeSpeedBack = 2.0f;
    private float shakeSpeed = 3.0f;

    public override void _Ready()
	{
		// timer pro shake
        shakeTimer = new Godot.Timer();

        var callable_shake = new Callable(this, "ResetShake");
        shakeTimer.Connect("timeout", callable_shake);
        shakeTimer.WaitTime = 0.2;
        shakeTimer.OneShot = true;
        AddChild(shakeTimer);
        shakeTimer.Stop();
    }

	public void Update(double delta)
	{
		if (!isActive) return;

        // shake interp
        actualShakeRot = actualShakeRot.Lerp(needShakeRot, shakeSpeed * (float)delta);
        applyNode.Rotation = actualShakeRot;
    }

	public void Init(Node3D newOwner, bool newStartActive = true)
	{
        applyNode = newOwner;
		newOwner.AddChild(this);
        SetActive(newStartActive);
	}

	public void FreeAll()
	{
        SetActive(false);
		shakeTimer.Stop();
		shakeTimer.QueueFree();
        shakeTimer.Free();
        shakeTimer = null;
	}

	public void SetActive(bool newActive) { isActive = newActive; }

	public void StartBasicShake(float newIntensity, float newTime, float newShakeSpeedTo, float newShakeSpeedBack,
        bool newApplyRotX = true, bool newApplyRotY = true, bool newApplyRotZ = true)
	{
		if (!isActive) return;
        if (shakeTimer == null) return;

        RandomNumberGenerator random = new RandomNumberGenerator();
        random.Randomize();

        FastNoiseLite noise = new FastNoiseLite();
        noise.Seed = (int)random.Randi();
        noise.FractalOctaves = 4;
        noise.Frequency = 5.0f;
        noise.FractalGain = 1.0f;

        random.Randomize();
        float a = noise.GetNoise1D(random.Randf());
        random.Randomize();
        float b = noise.GetNoise1D(random.Randf());
        random.Randomize();
        float c = noise.GetNoise1D(random.Randf());

        Vector3 noiseVector = new Vector3(0f, 0f, 0f);
        if (newApplyRotX) noiseVector.X = a;
        if (newApplyRotY) noiseVector.Y = b;
        if (newApplyRotZ) noiseVector.Z = c;

        needShakeRot = noiseVector * newIntensity;
        shakeSpeed = newShakeSpeedTo;
        shakeSpeedBack = newShakeSpeedBack;

        shakeTimer.WaitTime = newTime;
        shakeTimer.Start();

    }

	public void ResetShake()
	{
        needShakeRot = Vector3.Zero;
        shakeSpeed = shakeSpeedBack;
        shakeTimer.Stop();
    }
}
