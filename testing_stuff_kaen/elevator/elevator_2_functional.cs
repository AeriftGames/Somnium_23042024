using Godot;
using System;

public partial class elevator_2_functional : Node3D
{
    [Export] bool Enable = false;
    [Export] float PowerShake = 2.0f;
    [Export] float ShakeFadeMin = 1.0f;
    [Export] float ShakeFadeMax = 5.0f;

    private AnimationPlayer animDoorPlayer;
    private AnimationPlayer animPlayerElevatorMove;

    private bool isOpen = false;
    private bool isMoveDown = false;

    private bool isInDetectRange = false;
    public float DistanceFromPlayer = 100.0f;   //0-20
    InventoryObjectCamera invObjectCamera = null;
    RandomNumberGenerator Rng = new RandomNumberGenerator();

    [Export] public NodePath pickElevatorLevelCounter;
    private elevator_counter elevatorCounter;

    private Node3D cabine = null;

    public override void _Ready()
    {
        base._Ready();
        animDoorPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animPlayerElevatorMove = GetNode<AnimationPlayer>("AnimationPlayerElevatorMove");
        cabine = GetNode<Node3D>("CabineSystem");

        if (pickElevatorLevelCounter.IsEmpty == false)
            elevatorCounter = GetNode<elevator_counter>(pickElevatorLevelCounter);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("test_door"))
            Open(!isOpen);

        if (Input.IsActionJustPressed("test_elevator"))
            MoveElevator(!isMoveDown);

        if (cabine.Position.Y < -4.0f)
            elevatorCounter.SetLevel(-1);
        else
            elevatorCounter.SetLevel(0);

        GD.Print(cabine.Position.Y);
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

    public void MoveElevator(bool newMoveDown)
    {
        isMoveDown = newMoveDown;

        if (isMoveDown)
        {
            animPlayerElevatorMove.Play("MoveDown");
        }
        else
        {
            animPlayerElevatorMove.PlayBackwards("MoveDown");
        }
    }

    public void ShakeCabineStop() { DoCameraShake(1.0f); }
    public void ShakeDoorHit() { DoCameraShake(1.5f); }
    public void DoCameraShake(float mulValue)
    {
        // calculate Distance
        if (invObjectCamera != null)
        {
            if (CGameMaster.GM.GetGame().GetFPSCharacterOld() == null) return;

            // cam shake
            DistanceFromPlayer =
                GlobalPosition.DistanceTo(invObjectCamera.GetCharacterOwner().GlobalPosition);
        }

        float procent_distance = 100.0f - (DistanceFromPlayer / 0.2f);
        float final = 0.02f * procent_distance * PowerShake * mulValue;

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
