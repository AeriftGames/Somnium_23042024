using Godot;
using System;

public partial class FPSCharacterMoveAnim : FpsCharacterBase
{
    private CCharacterHeadBobComponent HeadBobComponent = null;
    private CCharacterLeanComponent LeanComponent = null;
    private CCharacterJumpLandEffectComponent JumpLandEffectComponent = null;
    private CCharacterCameraShakeComponent CameraShakeComponent = null;

    public CCharacterHeadBobComponent GetCharacterHeadBobComponent() { return HeadBobComponent; }
    public CCharacterLeanComponent GetCharacterLeanComponent() { return LeanComponent; }
    public CCharacterJumpLandEffectComponent GetJumpLandEffectComponent() { return JumpLandEffectComponent; }
    public CCharacterCameraShakeComponent GetCCharacterCameraShakeComponent() { return CameraShakeComponent; }

    public override void _Ready()
    {
        base._Ready();

        HeadBobComponent = GetBaseComponents().GetNode<CCharacterHeadBobComponent>("BaseHeadBobComponent");
        HeadBobComponent.PostInit(this);

        LeanComponent = GetBaseComponents().GetNode<CCharacterLeanComponent>("BaseLeanComponent");
        LeanComponent.PostInit(this);

        JumpLandEffectComponent = GetBaseComponents().GetNode<CCharacterJumpLandEffectComponent>
            ("BaseJumpLandEffectComponent");
        JumpLandEffectComponent.PostInit(this);

        CameraShakeComponent = GetBaseComponents().GetNode<CCharacterCameraShakeComponent>("BaseCameraShakeComponent");
        CameraShakeComponent.PostInit(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        HeadBobComponent.Update(delta);
        LeanComponent.InputUpdate(delta);
        JumpLandEffectComponent.Update(delta);
    }
}
