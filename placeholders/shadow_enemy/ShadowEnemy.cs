using Godot;
using System;

public partial class ShadowEnemy : Node3D
{
    [Export] float EnemyTimerTick = 0.75f;

    AudioStreamPlayer3D AudioStreamPlayer3D_Moving = null;

    InventoryObjectCamera invObjectCamera = null;

    private bool isInDetectRange = false;
    private bool isInDamageRange = false;

    private Timer EnemyTickTimer = null;

    RandomNumberGenerator Rng = new RandomNumberGenerator();

    public float DistanceFromPlayer = 100.0f;   //0-20

    public override void _Ready()
    {
        base._Ready();

        AudioStreamPlayer3D_Moving = GetNode<AudioStreamPlayer3D>("%AudioStreamPlayer3D_Moving");

        EnemyTickTimer = new Timer();
        AddChild(EnemyTickTimer);
        EnemyTickTimer.Connect("timeout", new Callable(this, "EnemyTick"));
        EnemyTickTimer.WaitTime = EnemyTimerTick;
        EnemyTickTimer.OneShot = false;
        EnemyTickTimer.Stop();

    }

    public override void _Process(double delta)
    {
        base._Process(delta);

    }

    public float GetRandomPitch(float new_min, float new_max) { return Rng.RandfRange(new_min, new_max); }

    public async void PlayMovingSound()
    {
        await ToSignal(GetTree().CreateTimer(Rng.RandfRange(0.0f, 0.25f)), "timeout");

        AudioStreamPlayer3D_Moving.PitchScale = GetRandomPitch(0.4f, 0.8f);
        AudioStreamPlayer3D_Moving.Play();
    }

    public void EnemyTick()
    {
        //GD.Print("EnemyTick");

        if (invObjectCamera == null) return;

        if (isInDetectRange)
        {
            DistanceFromPlayer =
                GlobalPosition.DistanceTo(invObjectCamera.GetCharacterOwner().GlobalPosition);

            //GD.Print(DistanceFromPlayer);
            //0.2 = jedno procento
            float procent_distance = 100.0f - (DistanceFromPlayer / 0.2f);
            GD.Print(procent_distance);
            //0.01-0.12
            float final = (0.15f / 100.0f) * procent_distance;
            invObjectCamera.GetHeadDangerShakeSystem().ApplyUserParamShake(final, Rng.RandfRange(1.0f, 5.0f));
            PlayMovingSound();
        }

        if (isInDamageRange)
        {
            FPSCharacter_Inventory player = invObjectCamera.GetCharacterOwner() as FPSCharacter_Inventory;
            if (player != null)
            {
                player.GetCharacterHealthComponent().RemoveHealth(10.0f);
            }
        }
    }

    public void _on_detect_player_area_body_entered(Node3D body)
    {
        FPSCharacter_Inventory player = body as FPSCharacter_Inventory;
        if (player != null)
        {
            
            invObjectCamera = player.GetObjectCamera() as InventoryObjectCamera;
            isInDetectRange = true;
            EnemyTickTimer.Start();
        }
    }

    public void _on_detect_player_area_body_exited(Node3D body)
    {
        FPSCharacter_Inventory player = body as FPSCharacter_Inventory;
        if (player != null)
        {
            invObjectCamera = null;
            isInDetectRange = false;
            EnemyTickTimer.Stop();
        }
    }

    public void _on_damage_player_area_body_entered(Node3D body)
    {
        if (isInDetectRange)
            isInDamageRange = true;
    }
    public void _on_damage_player_area_body_exited(Node3D body)
    {
        isInDamageRange = false;
    }
}
