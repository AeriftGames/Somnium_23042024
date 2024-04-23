using Godot;
using System;

/*
 * misto pro zlepseni = zvuk prvniho, mozna i posledniho kroku (ale neni to nutne)
 */

public partial class CCharacterWalkEffectComponent : CBaseComponent
{
    [Export] private float TOP_ANIMSPEED_WALK = 1.9f;
    [Export] private float TOP_ANIMSPEED_CROUCH = 2.4f;
    [Export] private float TOP_ANIMSPEED_CROUCH_EXTRA = 2.8f;
    [Export] private float TOP_ANIMSPEED_RUNNING = 1.7f;
    [Export] private bool ENABLE_ANIM = true;
    [Export] public bool ENABLE_SWAY = true;

    [ExportGroupAttribute("Sway Settings")]
    [Export] public float SwayAmount = 0.75f;
    [Export] public float SwayLerpSpeed = 2.5f;

    [ExportGroupAttribute("Audio Settings")]
    [Export] public float FootstepsVolumeDBInWalk = -2.0f;
    [Export] public float FootstepsVolumeDBInSprint = 1.0f;
    [Export] public float FootstepsVolumeDBInCrouch = -7.0f;
    [Export] public float FootstepsVolumeDBInCrouchExtra = -15.0f;
    [Export] public float FootstepsAudioPitch = 0.75f;

    private AnimationPlayer WalkBobAnimationPlayer = null;
    private AudioStreamPlayer AudioStreamPlayerFootsteps = null;

    private Node3D HeadBobNode = null;
    private Node3D CameraSway;

    private Vector3 workRot = Vector3.Zero;
    private all_material_surfaces AllMaterialSurfaces = null;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        WalkBobAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer_HeadBobMovement");
        AudioStreamPlayerFootsteps = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Footsteps");

        HeadBobNode = ourCharacterBase.GetNode<Node3D>("%HeadBob");
        CameraSway = ourCharacterBase.GetCharacterLookComponent().GetCameraSway();

        // nacteni vsech dat material surfaces
        AllMaterialSurfaces =
            (all_material_surfaces)GD.Load("res://player/material_surface/all_material_surfaces.tres");

        WalkBobAnimationPlayer.Play("Walk");
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        SetAnimationSpeed(GetCharacterSpeed());

        if (ENABLE_SWAY)
        {
            Vector3 workDir = new Vector3(ourCharacterBase.GetCharacterMovementComponent().GetInputDir().Normalized().Y *
                ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed(),
                0.0f, -ourCharacterBase.GetCharacterMovementComponent().GetInputDir().Normalized().X *
                ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed());

            workRot = workRot.Lerp(workDir.Normalized() * (0.05f * SwayAmount), (float)delta * 2.5f);
            workRot.Y = 0;

            CameraSway.Rotation = new Vector3(workRot.X, CameraSway.Rotation.Y, workRot.Z);
        }
    }

    private float GetCharacterSpeed() 
    {
        float speed = 0.0f;
        if (ourCharacterBase != null) 
        {
            if(ourCharacterBase.GetCharacterMovementComponent() != null)
            { speed = ourCharacterBase.GetCharacterMovementComponent().GetRealUnroundedSpeed(); }
        }

        return speed;
    }

    private void SetAnimationSpeed(float newSpeed)
    {
        var alpha = Mathf.Remap(newSpeed, 0.0f,
            ourCharacterBase.GetCharacterMovementComponent().SPEED_WALK, 0.0f, 1.0f);

        if (ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor() == false) 
        { WalkBobAnimationPlayer.SpeedScale = 0.0f; return; }

        if(ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "WalkingPlayerState")
        { WalkBobAnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TOP_ANIMSPEED_WALK, alpha); }
        else 
        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "CrouchMovePlayerState")
        { WalkBobAnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TOP_ANIMSPEED_CROUCH, alpha); }
        else
        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "CrouchActivePlayerState")
        { WalkBobAnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TOP_ANIMSPEED_CROUCH_EXTRA, alpha); }
        else
        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
        { WalkBobAnimationPlayer.SpeedScale = Mathf.Lerp(0.0f, TOP_ANIMSPEED_RUNNING, alpha); }
    }

    // vola se skrze animaci
    public void PlayFootStepNow()
    {
        FPSCharacterAction characterAction = ourCharacterBase as FPSCharacterAction;
        if (characterAction == null) return;

        float volume = 0.0f;

        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "WalkingPlayerState")
            volume = FootstepsVolumeDBInWalk;
        else if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
            volume = FootstepsVolumeDBInSprint;
        else if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "CrouchMovePlayerState")
            volume = FootstepsVolumeDBInCrouch;
        else if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "CrouchActivePlayerState")
            volume = FootstepsVolumeDBInCrouchExtra;

        PlayFootstepSound(volume,FootstepsAudioPitch);
    }

    public async void PlayFootstepSound(float addOffsetVolume = 0.0f, float addOffsetPitch = 0.0f)
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Detect materal surface name and play specific audio set of footsteps
        all_material_surfaces.EMaterialSurface materialSurface =
            AllMaterialSurfaces.GetMaterialSurfaceFromGroup(UniversalFunctions.DetectSurfaceMaterialOfFloor(
                ourCharacterBase, ourCharacterBase.GlobalPosition + (Vector3.Up * 0.3f)));

        //GD.Print(materialSurface);

        if (materialSurface != all_material_surfaces.EMaterialSurface.None)
        {
            // Play random footsteps sound by material surface
            UniversalFunctions.PlayRandomSound(
                AudioStreamPlayerFootsteps,
                AllMaterialSurfaces.GetAudioArray(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep),
                AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep) + addOffsetVolume,
                AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep) + addOffsetPitch);
        }
    }
}
