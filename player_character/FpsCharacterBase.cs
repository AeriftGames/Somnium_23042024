using Godot;
using System;

public partial class FpsCharacterBase : CharacterBody3D
{
	private CCharacterMovementComponent CharacterMovementComponent;
    private CCharacterLookComponent CharacterLookComponent;
    private CCharacterCrouchComponent CharacterCrouchComponent;

    public override void _Ready()
    {
        base._Ready();

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

        if(Input.IsActionJustPressed("Crunch"))
            CharacterCrouchComponent.ToggleCrouch();
	}
}
