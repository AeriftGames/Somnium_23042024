using Godot;
using System;

public partial class FpsCharacterBase : CharacterBody3D
{
	private CCharacterMovementComponent CharacterMovementComponent;
    private CCharacterLookComponent CharacterLookComponent;
    private CCharacterCrouchComponent CharacterCrouchComponent;

    public CCharacterMovementComponent GetCharacterMovementComponent() { return CharacterMovementComponent; }
    public CCharacterLookComponent GetCharacterLookComponent() { return CharacterLookComponent; }
    public CCharacterCrouchComponent GetCharacterCrouchComponent() { return this.CharacterCrouchComponent; }

    public override void _Ready()
    {
        base._Ready();

        CGameMaster.GM.GetGame().SetFPSCharacterBase(this);

        CharacterMovementComponent = GetNode<CCharacterMovementComponent>("BaseComponents/BaseMovementComponent");
        CharacterMovementComponent.PostInit(this);

        CharacterLookComponent = GetNode<CCharacterLookComponent>("BaseComponents/BaseLookComponent");
        CharacterLookComponent.PostInit(this);

        CharacterCrouchComponent = GetNode<CCharacterCrouchComponent>("BaseComponents/BaseCrouchComponent");
        CharacterCrouchComponent.PostInit(this);
    }

    public override void _PhysicsProcess(double delta)
	{
        CharacterLookComponent.UpdateLook(delta);

		CharacterMovementComponent.UpdateMove(delta);

        CharacterMovementComponent.CheckAndApplyJump("Jump");
        CharacterCrouchComponent.CheckAndApplyCrouch("Crunch");

        CharacterMovementComponent.ApplyWorkVelocity();

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Character Velocity",
            new Vector3(float.Round(GetRealVelocity().X,1),
            float.Round(GetRealVelocity().Y, 1), 
            float.Round(GetRealVelocity().Z, 1)).ToString(),0);

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Character Speed",
            GetCharacterMovementComponent().GetRealSpeed().ToString(), 1);
    }
}
