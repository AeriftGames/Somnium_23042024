using Godot;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using static all_material_surfaces;

public partial class CCharacterJumpLandEffectComponent : CBaseComponent
{

    [ExportGroupAttribute("Shake Settings")]
    [Export] public float JumpShakeStrenght = 1.0f;
    [Export] public float JumpShakeFade = 9.0f;
    [Export] public float LandShakeStrenght = 1.0f;
    [Export] public float LandShakeFade = 9.0f;
    [ExportGroupAttribute("Audio Settings")]
    [Export] public Godot.Collections.Array<AudioStream> JumpingSounds;
    [Export] public float JumpingVolumeDB = -5f;
    [Export] public float JumpingAudioPitch = 1.0f;
    [Export] public float JumpingAudioPitchOffset = 0.2f;

    AnimationPlayer PlayerAnim;
    AudioStreamPlayer PlayerAudio;

    public all_material_surfaces AllMaterialSurfaces = null;

    private Node3D CameraJump = null;
    private Node3D CameraLand = null;

    //
    private float lastYPosFallingStart = 0.0f;
    private float lastYPosFallingEnd = 0.0f;

    //
    private CSpring LandingSpring = new CSpring();
    private float lastVel = 0.0f;
    private bool isOnGround = false;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        // nacteni vsech dat material surfaces
        AllMaterialSurfaces =
            (all_material_surfaces)GD.Load("res://player/material_surface/all_material_surfaces.tres");

        PlayerAnim = GetNode<AnimationPlayer>("AnimationPlayer_JumpLand");
        PlayerAudio = GetNode<AudioStreamPlayer>("AudioStreamPlayer_JumpLand");

        CameraJump = ourCharacterBase.GetCharacterLookComponent().GetCameraJump();
        CameraLand = ourCharacterBase.GetCharacterLookComponent().GetCameraLand();
    }

    public void Update(double delta)
    {
        // Zde probiha update springu z nehoz vypocitame fyzikalne landing hodnotu pro lerp pozice

        LandingSpring.Update(delta);

        if ( !isOnGround && ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor())
        {
            isOnGround = true;
            LandingSpring.Reset();
            LandingSpring.Velocity = -lastVel;
        }
        else if (!ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor())
        { isOnGround = false; }

        lastVel = ourCharacterBase.Velocity.Y;

        // lerp pro landing - bacha vzdy na offset (-Vector se meni vzdy kdyz budeme menit hodnoty) 
        CameraLand.Position = CameraLand.Position.Lerp((LandingBounce() / 20.0f) - new Vector3(0,0.45f,0), 5.0f * (float)delta);
        //GD.Print(CameraLand.Position);
    }

    public void ApplyEffectJump()
    {
        PlayerAnim.Play("Jump");

        RandomNumberGenerator a = new RandomNumberGenerator();
        a.Randomize();
        float PitchScale = a.RandfRange(JumpingAudioPitch - (JumpingAudioPitchOffset / 2),
                JumpingAudioPitch + (JumpingAudioPitchOffset / 2));

        // play sounds
        UniversalFunctions.PlayRandomSound(PlayerAudio, JumpingSounds, JumpingVolumeDB, PitchScale);

        // Pokud mame komponentu pro Shake - provedeme jej
        FPSCharacterMoveAnim FPSMoveAnim = ourCharacterBase as FPSCharacterMoveAnim;
        if(FPSMoveAnim != null)
        { FPSMoveAnim.GetCharacterCameraShakeComponent().ApplyUserParamShake(JumpShakeStrenght,JumpShakeFade);}
    }

    public async void ApplyEffectLand()
    {
        // calculate amount
        CalculateAmountLanding();

        await ToSignal(GetTree(), "physics_frame");

        // Detect materal surface name and play specific audio set of footsteps
        EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(
            UniversalFunctions.DetectSurfaceMaterialOfFloor(ourCharacterBase,
            ourCharacterBase.GlobalPosition+(Vector3.Up*0.3f)));

        if (materialSurface != EMaterialSurface.None)
        {
            // Play random sound
            UniversalFunctions.PlayRandomSound(
                PlayerAudio,
                AllMaterialSurfaces.GetAudioArray(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing) - 8,
                AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing) - 0.1f);
        };

        // Pokud mame komponentu pro Shake - provedeme jej
        FPSCharacterMoveAnim FPSMoveAnim = ourCharacterBase as FPSCharacterMoveAnim;
        if (FPSMoveAnim != null)
        { FPSMoveAnim.GetCharacterCameraShakeComponent().ApplyUserParamShake(LandShakeStrenght, LandShakeFade); }
    }

    public void SetStartFallingNow() { lastYPosFallingStart = ourCharacterBase.GlobalPosition.Y; }
    public void CalculateAmountLanding()
    {
        if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 3.0f)
        { PlayerAnim.Play("CameraLandMedium_4"); }
        else
        if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 2.0f)
        { PlayerAnim.Play("CameraLandMedium_2"); }
    }

    private Vector3 LandingBounce() { return new Vector3(0.0f, -LandingSpring.Value, 0.0f); }

}
