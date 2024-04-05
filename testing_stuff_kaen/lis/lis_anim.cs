using Godot;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public partial class lis_anim : Node3D
{
    [Export] bool Enable = false;
    [Export] float DelayMsStartAnim = 1;
    [Export] float PowerShake = 2.0f;
    [Export] float ShakeFadeMin = 1.0f;
    [Export] float ShakeFadeMax = 5.0f;

    AnimationPlayer AnimPlayer;
    AudioStreamPlayer3D AudioStreamPlayer_Touch;

    InventoryObjectCamera invObjectCamera = null;
    BenchmarkCameraBody benchmarkCamBody = null;
    FPSCharacterAction charAction = null;

    private bool isInDetectRange = false;
    public float DistanceFromPlayer = 100.0f;   //0-20

    RandomNumberGenerator Rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();

        AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        AudioStreamPlayer_Touch = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D_Touch");

        StartGame();
    }

    public async void StartGame()
    {
        await ToSignal(CGameMaster.GM.GetMasterSignals(), CMasterSignals.SignalName.GameStart);

        await ToSignal(GetTree().CreateTimer(DelayMsStartAnim), "timeout");
        if(Enable)
            AnimPlayer.Play("work");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

    }

    public float GetRandomPitch(float new_min, float new_max) { return Rng.RandfRange(new_min, new_max); }

    public async void PlayTouchSound()
    {
        if (Enable && isInDetectRange)
        {
            //audio
            await ToSignal(GetTree().CreateTimer(Rng.RandfRange(0.02f, 0.25f)), "timeout");
            AudioStreamPlayer_Touch.PitchScale = GetRandomPitch(0.4f, 0.8f);
            AudioStreamPlayer_Touch.Play();

            // calculate Distance
            if (invObjectCamera != null)
            {
                if (CGameMaster.GM.GetGame().GetFPSCharacterOld() == null) return;

                // cam shake
                DistanceFromPlayer =
                    GlobalPosition.DistanceTo(invObjectCamera.GetCharacterOwner().GlobalPosition);
            }
            else if (benchmarkCamBody != null)
            {
                DistanceFromPlayer =
                    GlobalPosition.DistanceTo(benchmarkCamBody.GlobalPosition);
            }
            else if (charAction != null)
            {
                DistanceFromPlayer =
                    GlobalPosition.DistanceTo(charAction.GlobalPosition);
            }

            //GD.Print(DistanceFromPlayer);
            //0.2 = jedno procento
            float procent_distance = 100.0f - (DistanceFromPlayer / 0.2f);
            //GD.Print(procent_distance);
            float final = 0.02f * procent_distance * PowerShake;
            //GD.Print(final);

            if (invObjectCamera != null)
                invObjectCamera.GetHeadDangerShakeSystem().ApplyUserParamShake(final, Rng.RandfRange(ShakeFadeMin, ShakeFadeMax));
            else if (benchmarkCamBody != null)
                benchmarkCamBody.ApplyUserParamShake(final, Rng.RandfRange(ShakeFadeMin, ShakeFadeMax));
            else if (charAction != null)
                charAction.GetCharacterCameraShakeComponent().ApplyUserParamShake(final, Rng.RandfRange(ShakeFadeMin, ShakeFadeMax));
        }
    }

    public void _on_detect_player_area_body_entered(Node3D body)
    {
        //GD.Print(body);
        FPSCharacter_Inventory player = body as FPSCharacter_Inventory;
        if (player != null)
        {

            invObjectCamera = player.GetObjectCamera() as InventoryObjectCamera;
            isInDetectRange = true;
            return;
        }

        BenchmarkCameraBody benchCam = body as BenchmarkCameraBody;
        if (benchCam != null)
        {
            benchmarkCamBody = benchCam;
            isInDetectRange = true;
            return;
        }

        FPSCharacterAction chAction = body as FPSCharacterAction;
        if (chAction != null)
        {
            charAction = chAction;
            isInDetectRange = true;
            return;
        }
    }

    public void _on_detect_player_area_body_exited(Node3D body)
    {
        //GD.Print(body);
        FPSCharacter_Inventory player = body as FPSCharacter_Inventory;
        if (player != null)
        {
            invObjectCamera = null;
            isInDetectRange = false;
            return;
        }

        BenchmarkCameraBody benchCam = body as BenchmarkCameraBody;
        if (benchCam != null)
        {
            benchmarkCamBody = null;
            isInDetectRange = false;
            return;
        }

        FPSCharacterAction chAction = body as FPSCharacterAction;
        if (chAction != null)
        {
            charAction = null;
            isInDetectRange = false;
            return;
        }
    }
}
