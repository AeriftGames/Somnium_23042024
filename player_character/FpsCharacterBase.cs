using Godot;
using System;

public partial class FpsCharacterBase : CharacterBody3D
{
    private CCharacterMovementComponent CharacterMovementComponent;
    private CCharacterLookComponent CharacterLookComponent;
    private CCharacterCrouchComponent CharacterCrouchComponent;
    private CCharacterFlashlightComponent CharacterFlashlightComponent;

    public CCharacterMovementComponent GetCharacterMovementComponent() { return CharacterMovementComponent; }
    public CCharacterLookComponent GetCharacterLookComponent() { return CharacterLookComponent; }
    public CCharacterCrouchComponent GetCharacterCrouchComponent() { return CharacterCrouchComponent; }
    public CCharacterFlashlightComponent GetCharacterFlashlightComponent() { return CharacterFlashlightComponent; }


    private CStateMachine CharacterStateMachine;
    public CStateMachine GetCharacterStateMachine() { return CharacterStateMachine; }

    public Node3D GetBaseComponents() { return GetNode<Node3D>("BaseComponents"); }

    // predelat a dat na lepsi misto - ted jen na test
    public double movementAnimationLastTime = 0.0f;

    public override void _Ready()
    {
        base._Ready();

        CGameMaster.GM.GetGame().SetFPSCharacterBase(this);

        CharacterStateMachine = GetNode<CStateMachine>("PlayerStateMachine");
        CharacterStateMachine.PostInit();

        CharacterMovementComponent = GetNode<CCharacterMovementComponent>("BaseComponents/BaseMovementComponent");
        CharacterMovementComponent.PostInit(this);

        CharacterLookComponent = GetNode<CCharacterLookComponent>("BaseComponents/BaseLookComponent");
        CharacterLookComponent.PostInit(this);

        CharacterCrouchComponent = GetNode<CCharacterCrouchComponent>("BaseComponents/BaseCrouchComponent");
        CharacterCrouchComponent.PostInit(this);
    }

    public override void _PhysicsProcess(double delta)
	{
        GetCharacterLookComponent().UpdateLook(delta);

       GetCharacterMovementComponent().UpdateMove(delta);

        GetCharacterMovementComponent().CheckAndApplyJump("Jump");
        GetCharacterCrouchComponent().CheckAndApplyCrouch("Crunch");

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
}
