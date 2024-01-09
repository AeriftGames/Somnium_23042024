using Godot;
using System;

public partial class FpsCharacterBase : CharacterBody3D
{
    private CCharacterMovementComponent CharacterMovementComponent;
    private CCharacterLookComponent CharacterLookComponent;
    private CCharacterFovComponent CharacterFovComponent;
    private CCharacterCrouchComponent CharacterCrouchComponent;
    private CCharacterFlashlightComponent CharacterFlashlightComponent;

    public CCharacterMovementComponent GetCharacterMovementComponent() { return CharacterMovementComponent; }
    public CCharacterLookComponent GetCharacterLookComponent() { return CharacterLookComponent; }
    public CCharacterFovComponent GetCharacterFovComponent() { return CharacterFovComponent; }
    public CCharacterCrouchComponent GetCharacterCrouchComponent() { return CharacterCrouchComponent; }
    public CCharacterFlashlightComponent GetCharacterFlashlightComponent() { return CharacterFlashlightComponent; }


    private CStateMachine CharacterStateMachine;
    public CStateMachine GetCharacterStateMachine() { return CharacterStateMachine; }

    public Node3D GetBaseComponents() { return GetNode<Node3D>("BaseComponents"); }

    private CollisionShape3D CharacterCollisionShape = null;
    public CollisionShape3D GetCharacterCollisionShape() { return CharacterCollisionShape; }

    // predelat a dat na lepsi misto - ted jen na test
    public double movementAnimationLastTime = 0.0f;

    public override void _Ready()
    {
        base._Ready();

        CGameMaster.GM.GetGame().SetFPSCharacterBase(this);

        CharacterCollisionShape = GetNode<CollisionShape3D>("CharacterCollisionShape");

        CharacterStateMachine = GetNode<CStateMachine>("PlayerStateMachine");
        CharacterStateMachine.PostInit();

        CharacterMovementComponent = GetBaseComponents().GetNode<CCharacterMovementComponent>("BaseMovementComponent");
        CharacterMovementComponent.PostInit(this);

        CharacterLookComponent = GetBaseComponents().GetNode<CCharacterLookComponent>("BaseLookComponent");
        CharacterLookComponent.PostInit(this);

        CharacterFovComponent = GetBaseComponents().GetNode<CCharacterFovComponent>("BaseFovComponent");
        CharacterFovComponent.PostInit(this);

        CharacterCrouchComponent = GetBaseComponents().GetNode<CCharacterCrouchComponent>("BaseCrouchComponent");
        CharacterCrouchComponent.PostInit(this);
    }

    public override void _PhysicsProcess(double delta)
	{
        GetCharacterLookComponent().UpdateLook(delta);

        ApplyMovementInputActions(delta);  // virtual for sprint crouch walk speed

        // Final apply velocity
        GetCharacterMovementComponent().ApplyWorkVelocity();

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Character Position",
            new Vector3(float.Round(GlobalPosition.X, 1),
            float.Round(GlobalPosition.Y, 1),
            float.Round(GlobalPosition.Z, 1)).ToString(), 0);

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Character Velocity",
            new Vector3(float.Round(GetRealVelocity().X,1),
            float.Round(GetRealVelocity().Y, 1), 
            float.Round(GetRealVelocity().Z, 1)).ToString(),1);

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Character Speed",
            GetCharacterMovementComponent().GetRealSpeed().ToString(), 2);
    }

    public virtual void ApplyMovementInputActions(double delta)
    {
        // SPEED
        if (Input.IsActionPressed("Sprint") && GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == false)
            GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_SPRINT);

        else if (GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == true && 
            GetCharacterCrouchComponent().GetIsCrouchExtra() == false)
            GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_CROUCH);

        else if (GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == true && 
            GetCharacterCrouchComponent().GetIsCrouchExtra() == true)
            GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_CROUCH_DYNAMIC);

        else if (GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == false)
            GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_WALK);

        // MOVEMENT
        GetCharacterMovementComponent().UpdateMove(delta);

        // JUMP AND CROUCH
        GetCharacterMovementComponent().CheckAndApplyJump("Jump");
        GetCharacterCrouchComponent().CheckAndApplyCrouch("Crunch");
    }
}
