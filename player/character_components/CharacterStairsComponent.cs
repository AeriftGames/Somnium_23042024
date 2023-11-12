using Godot;
using System;

public partial class CharacterStairsComponent : Node3D
{
    // 0.4 zacatek schodu
    // 0.5 konec schodu

    [Export] public bool EnableStairsDetectEffect = true;
    [Export] public bool EnableDebugPrint = false;

    [Export] public bool EnableCharacterEffects = true;
    [ExportGroupAttribute("CharacterEffects")]
    [Export] public bool EnableMoveSpeedEffect = true;
    [ExportSubgroup("Move Speed Effect")]
    [Export] public float MoveSpeedStandPercent = 80.0f;
    [Export] public float MoveSpeedSprintPercent = 80.0f;
    [Export] public float MoveSpeedCrouchPercent = 90.0f;


    private FPSCharacter_Inventory inventoryCharacter = null;

    private RayCast3D rayCast3D = null;

    private float LastStairDistance = 0.0f;
    private bool FirstStep = true;
    private bool isAlreadyEndStep = true;
    private bool isMoveOnStairs = false;

    public override void _Ready()
    {
        base._Ready();

        rayCast3D = GetNode<RayCast3D>("RayCast3D");
    }

    public void StartInit(FPSCharacter_Inventory ownCharacter)
    {
        inventoryCharacter = ownCharacter;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (GameMaster.GM.GetIsQuitting()) return;
        if (!EnableStairsDetectEffect) return;

        isMoveOnStairs = false;

        if (rayCast3D.IsColliding())
        {
            Node3D hitnode = rayCast3D.GetCollider() as Node3D;

            if (hitnode == null) return;
            if(!GameMaster.GM.GetFPSCharacter().IsOnFloor()) return;

            float distance = Mathf.Abs(rayCast3D.GlobalPosition.DistanceTo(rayCast3D.GetCollisionPoint()));
            float rozdil = distance - LastStairDistance;
            LastStairDistance = distance;

            if (hitnode.IsInGroup("tag_stairs") && GameMaster.GM.GetFPSCharacter().IsOnFloor())
            {
                isMoveOnStairs = true;

                if (Mathf.Abs(rozdil) > 0.02f)
                {
                    isAlreadyEndStep = false;

                    if (rozdil > 0.02f)
                    {
                        SetMoveOnStairs(false);
                    }
                    else
                    {
                        SetMoveOnStairs(true);
                    }
                }
            }
            else
            {
                FirstStep = true;

                if (!isAlreadyEndStep)
                    EndStep(rozdil);
            }

            FirstStep = true;
        }

        //GD.Print(isMoveOnStairs);

        ApplyEffects((float)delta);
    }

    public void EndStep(float new_rozdil)
    {
        isAlreadyEndStep = true;

        if (GameMaster.GM.GetFPSCharacter().IsOnFloor())
        {
            if(new_rozdil > 0.02f)
            {
                if(EnableDebugPrint)
                {
                    GD.Print("last krok");
                    //GD.Print(new_rozdil);
                }
            }
        }
    }

    public void SetMoveOnStairs(bool newMoveOnStairsUp)
    {
        if (newMoveOnStairsUp)
        {
            if(EnableDebugPrint)
                GD.Print("krok nahoru");
            /*
            InventoryObjectCamera a = GameMaster.GM.GetFPSCharacter().GetObjectCamera() as InventoryObjectCamera;
            a.GetHeadStairsBobComponent().ApplyEffectStairStep(true, 1.0f);
            */
        }
        else
        {
            if (EnableDebugPrint)
                GD.Print("krok dolu");
            /*
            InventoryObjectCamera a = GameMaster.GM.GetFPSCharacter().GetObjectCamera() as InventoryObjectCamera;
            a.GetHeadStairsBobComponent().ApplyEffectStairStep(false, 1.0f);
            */
        }
    }

    public void ApplyEffects(float delta)
    {
        if(EnableCharacterEffects)
        {
            if(EnableMoveSpeedEffect)
                ApplyMoveSpeedEffect(delta);
        }
    }

    public void ApplyMoveSpeedEffect(float delta)
    {
        if (isMoveOnStairs)
        {
            GameMaster.GM.GetFPSCharacter().MoveSpeedInStand =
                (GameMaster.GM.GetFPSCharacter().DefaultMoveSpeedInStand / 100.0f) * MoveSpeedStandPercent;

            GameMaster.GM.GetFPSCharacter().MoveSpeedInSprint =
                (GameMaster.GM.GetFPSCharacter().DefaultMoveSpeedInSprint / 100.0f) * MoveSpeedSprintPercent;

            GameMaster.GM.GetFPSCharacter().MoveSpeedInCrunch =
                (GameMaster.GM.GetFPSCharacter().DefaultMoveSpeedInCrunch / 100.0f) * MoveSpeedCrouchPercent;

            GameMaster.GM.GetFPSCharacter().LerpSpeedCameraY = 6.0f;

            FPSCharacter_Inventory b = GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
            b.FootStepLengthInWalk = 0.9f;
            b.FootStepLengthInSprint = 1.0f;
            b.FootStepLengthInCrouch = 0.6f;
            b.FootstepsAudioPitch = 0.15f;

            //
            InventoryObjectCamera a = GameMaster.GM.GetFPSCharacter().GetObjectCamera() as InventoryObjectCamera;
            a.GetHeadBobSystem().headBobbingWalkValue = 0.2f;
            a.GetHeadBobSystem().headBobbingSprintValue = 0.25f;
            a.GetHeadBobSystem().headBobRotDegSprintValue = 1.5f;

            a.GetHeadBobSystem().headBobRotDegWalkValue = 0.8f;

            a.GetHeadBobSystem().WalkCameraLerpHeight = 0.2f;
            a.GetHeadBobSystem().RunCameraLerpHeight = 0.4f;
        }
        else
        {
            GameMaster.GM.GetFPSCharacter().MoveSpeedInStand = GameMaster.GM.GetFPSCharacter().DefaultMoveSpeedInStand;
            GameMaster.GM.GetFPSCharacter().MoveSpeedInSprint = GameMaster.GM.GetFPSCharacter().DefaultMoveSpeedInSprint;
            GameMaster.GM.GetFPSCharacter().MoveSpeedInCrunch = GameMaster.GM.GetFPSCharacter().DefaultMoveSpeedInCrunch;

            GameMaster.GM.GetFPSCharacter().LerpSpeedCameraY = 0.0f;

            FPSCharacter_Inventory b = GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
            b.FootStepLengthInWalk = 1.2f;
            b.FootStepLengthInSprint = 1.25f;
            b.FootStepLengthInCrouch = 0.85f;
            b.FootstepsAudioPitch = 0.0f;

            //
            InventoryObjectCamera a = GameMaster.GM.GetFPSCharacter().GetObjectCamera() as InventoryObjectCamera;
            a.GetHeadBobSystem().headBobbingWalkValue = a.GetHeadBobSystem().DefaultheadBobbingWalkValue;
            a.GetHeadBobSystem().headBobbingSprintValue = a.GetHeadBobSystem().DefaultheadBobbingSprintValue;
            a.GetHeadBobSystem().headBobRotDegSprintValue = a.GetHeadBobSystem().DefaultheadBobRotDegSprintValue;

            a.GetHeadBobSystem().headBobRotDegWalkValue = a.GetHeadBobSystem().DefaultheadBobRotDegWalkValue;

            a.GetHeadBobSystem().WalkCameraLerpHeight = a.GetHeadBobSystem().DefaultWalkCameraLerpHeight;
            a.GetHeadBobSystem().RunCameraLerpHeight = a.GetHeadBobSystem().DefaultRunCameraLerpHeight;


        }
    }
}
