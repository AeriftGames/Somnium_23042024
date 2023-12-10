using Godot;
using System;

public partial class FPSCharacterMoveAnim : FpsCharacterBase
{
    private CCharacterHeadBobComponent HeadBobComponent = null;
    private CCharacterLeanComponent LeanComponent = null;
    private CCharacterJumpLandEffectComponent JumpLandEffectComponent = null;
    private CCharacterCameraShakeComponent CameraShakeComponent = null;
    private CCharacterCameraZoomComponent CameraZoomComponent = null;
    private CCharacterBreathingEffectComponent BreathingEffectComponent = null;

    public CCharacterHeadBobComponent GetCharacterHeadBobComponent() { return HeadBobComponent; }
    public CCharacterLeanComponent GetCharacterLeanComponent() { return LeanComponent; }
    public CCharacterJumpLandEffectComponent GetJumpLandEffectComponent() { return JumpLandEffectComponent; }
    public CCharacterCameraShakeComponent GetCharacterCameraShakeComponent() { return CameraShakeComponent; }
    public CCharacterCameraZoomComponent GetCharacterCameraZoomComponent() { return CameraZoomComponent; }
    public CCharacterBreathingEffectComponent GetCharacterBreathingEffectComponent() { return BreathingEffectComponent; }

    public override void _Ready()
    {
        base._Ready();

        HeadBobComponent = GetBaseComponents().GetNode<CCharacterHeadBobComponent>("BaseHeadBobComponent");
        HeadBobComponent.PostInit(this);

        LeanComponent = GetBaseComponents().GetNode<CCharacterLeanComponent>("BaseLeanComponent");
        LeanComponent.PostInit(this);

        JumpLandEffectComponent = 
            GetBaseComponents().GetNode<CCharacterJumpLandEffectComponent>("BaseJumpLandEffectComponent");
        JumpLandEffectComponent.PostInit(this);

        CameraShakeComponent = GetBaseComponents().GetNode<CCharacterCameraShakeComponent>("BaseCameraShakeComponent");
        CameraShakeComponent.PostInit(this);

        CameraZoomComponent = GetBaseComponents().GetNode<CCharacterCameraZoomComponent>("BaseCameraZoomComponent");
        CameraZoomComponent.PostInit(this);

        BreathingEffectComponent = 
            GetBaseComponents().GetNode<CCharacterBreathingEffectComponent>("BaseBreathingEffectComponent");
        BreathingEffectComponent.PostInit(this);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        //
        CameraZoomComponent.Update(delta);
        BreathingEffectComponent.Update(delta);
        GetCharacterLookComponent().ApplyFinalFov();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        HeadBobComponent.Update(delta);
        LeanComponent.InputUpdate(delta);
        JumpLandEffectComponent.Update(delta);

        CameraZoomComponent.InputUpdate(delta);
    }
}
