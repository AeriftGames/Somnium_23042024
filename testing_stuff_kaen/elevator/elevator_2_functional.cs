using Godot;
using System;

public partial class elevator_2_functional : Node3D
{
    [Export] bool Enable = false;
    [Export] float PowerShake = 2.0f;
    [Export] float ShakeFadeMin = 1.0f;
    [Export] float ShakeFadeMax = 5.0f;

    private AnimationPlayer animDoorPlayer;

    private bool isOpen = false;

    private bool isInDetectRange = false;
    public float DistanceFromPlayer = 100.0f;   //0-20
    InventoryObjectCamera invObjectCamera = null;
    RandomNumberGenerator Rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        base._Ready();
        animDoorPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("test_door"))
            Open(!isOpen);
    }

    public void Open(bool newOpen)
    {
        isOpen = newOpen;

        if(isOpen)
        {
            animDoorPlayer.Play("DoorOpen");
        }
        else
        {
            animDoorPlayer.Play("DoorClose");
        }
    }

    public void ShakeDoorHit()
    {
        // calculate Distance
        if (invObjectCamera != null)
        {
            if (GameMaster.GM.GetFPSCharacter() == null) return;

            // cam shake
            DistanceFromPlayer =
                GlobalPosition.DistanceTo(invObjectCamera.GetCharacterOwner().GlobalPosition);
        }

        float procent_distance = 100.0f - (DistanceFromPlayer / 0.2f);
        float final = 0.02f * procent_distance * PowerShake;

        if (invObjectCamera != null)
            invObjectCamera.GetHeadDangerShakeSystem().ApplyUserParamShake(final, Rng.RandfRange(ShakeFadeMin, ShakeFadeMax));
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
            isInDetectRange = false;
            return;
        }
    }
}
