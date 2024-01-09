using Godot;
using System;

public partial class FPSCharacterAction : FPSCharacterMoveAnim
{
    private CCharacterFlashlightComponent FlashlightComponent = null;
    private CCharacterUseActionComponent UseActionComponent = null;
    
    private CCharacterStaminaComponent StaminaComponent = null;
    private CCharacterHealthComponent HealthComponent = null;

    public CCharacterFlashlightComponent GetFlashlightComponent() { return FlashlightComponent; }
    public CCharacterUseActionComponent GetUseActionComponent() { return UseActionComponent; }
    public CCharacterStaminaComponent GetStaminaComponent() { return StaminaComponent; }
    public CCharacterHealthComponent GetHealthComponent() { return HealthComponent; }

    public override void _Ready()
    {
        base._Ready();

        FlashlightComponent = GetBaseComponents().GetNode<CCharacterFlashlightComponent>("BaseFlashlightComponent");
        FlashlightComponent.PostInit(this);

        UseActionComponent = GetBaseComponents().GetNode<CCharacterUseActionComponent>("BaseUseActionComponent");
        UseActionComponent.PostInit(this);

        StaminaComponent = GetBaseComponents().GetNode<CCharacterStaminaComponent>("BaseStaminaComponent");
        StaminaComponent.PostInit(this);

        HealthComponent = GetBaseComponents().GetNode<CCharacterHealthComponent>("BaseHealthComponent");
        HealthComponent.PostInit(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GetUseActionComponent().Update(delta);
    }

    // override function (movement speed) with stamina system
    public override void ApplyMovementInputActions(double delta)
    {
        if(GetStaminaComponent() == null)
            base.ApplyMovementInputActions(delta);
        else
        {
            // SPEED
            if (Input.IsActionPressed("Sprint") && GetCharacterMovementComponent().GetIsOnFloor() &&
                GetCharacterCrouchComponent().GetIsCrouched() == false && GetStaminaComponent().GetStamina() > 0.1f )
            { GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_SPRINT); }

            else if (GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == true &&
                GetCharacterCrouchComponent().GetIsCrouchExtra() == false)
            { GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_CROUCH); }

            else if (GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == true &&
                GetCharacterCrouchComponent().GetIsCrouchExtra() == true)
            { GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_CROUCH_DYNAMIC); }

            else if (GetCharacterMovementComponent().GetIsOnFloor() && GetCharacterCrouchComponent().GetIsCrouched() == false)
            { GetCharacterMovementComponent().SetMoveSpeed(CCharacterMovementComponent.ESpeedMoveType.SPEED_WALK); }

            // MOVEMENT
            GetCharacterMovementComponent().UpdateMove(delta);

            // JUMP
            if(GetStaminaComponent().GetStamina() > 5.0f)
            { GetCharacterMovementComponent().CheckAndApplyJump("Jump"); }
            
            // CROUCH
            GetCharacterCrouchComponent().CheckAndApplyCrouch("Crunch");

            // FULLY STAMINA OUT - zadychani
            if (GetStaminaComponent().GetStamina() <= 0.1f && !GetStaminaComponent().GetStaminaExhaustActive())
            {
                GD.Print("STAMINA OUT");
                GetStaminaComponent().ApplyStaminaExhaustForTime(1.0f);
            }
        }
    }
}
