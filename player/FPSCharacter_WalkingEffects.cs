using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Threading;
using System.Threading.Tasks;
using static all_material_surfaces;

/*
 * *** FPSCharacter_WalkingEffects(0.2) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and provide extra walking effects
 * - simulation footsteps
 * - walking headbob lerping effects and sounds of footsteps
 * - landing camera lerp effect
 * - jumping sound effect
 * TODO - fix small amount walking/stop no sound - it is weird some times
 * TODO - fix (change) volume,pitch etc footsteps and velocity bobhead when player moves crouched
*/
public partial class FPSCharacter_WalkingEffects : FPSCharacter_BasicMoving
{
    AudioStreamPlayer AudioStreamPlayerFootsteps = null;
    AudioStreamPlayer AudioStreamPlayerJumpLand = null;
    AudioStreamPlayer AudioStreamPlayerCrouching = null;

    [ExportGroupAttribute("Footsteps Settings")]
    [Export] public float FootStepLengthInWalk = 1.2f;
    [Export] public float FootStepLengthInSprint = 1.25f;
    [Export] public float FootStepLengthInCrouch = 0.85f;

    [Export] public Array<AudioStream> FootstepSounds;
    [Export] public float FootstepsVolumeDBInWalk = -2.0f;
    [Export] public float FootstepsVolumeDBInSprint = 1.0f;
    [Export] public float FootstepsVolumeDBInCrouch = -7.0f;
    [Export] public float FootstepsAudioPitch = 0.75f;

    [ExportGroupAttribute("Landing Settings")]
    [Export] public float LandCameraLerpHeight = -0.4f;
    [Export] public float LandCameraLerpRotation = -0.1f;
    [Export] public float lerpLandingSpeedModifier = 3.0f;

    [Export] public float MiniHeightForLandingEffect = 0.35f;
    [Export] public float SmallHeightForLandingEffect = 1.2f;
    [Export] public float MediumHeightForLandingEffect = 2.5f;
    [Export] public float HighHeightForLandingEffect = 4.0f;    // For death is more than high
    [Export] public Array<AudioStream> miniHeightLandingSounds;
    [Export] public Array<AudioStream> smallHeightLandingSounds;
    [Export] public Array<AudioStream> mediumHeightLandingSounds;
    [Export] public Array<AudioStream> highHeightLandingSounds;
    [Export] public Array<AudioStream> deathHeightLandingSounds;
    [Export] public float LandingVolumeDB = -10.0f;

    [ExportGroupAttribute("Jumping Settings")]
    [Export] public Array<AudioStream> JumpingSounds;
    [Export] public float JumpingVolumeDB = -5f;
    [Export] public float JumpingAudioPitch = 1.0f;
    [Export] public float JumpingAudioPitchOffset = 0.2f;

    [ExportGroupAttribute("Leaning Settings")]
    [Export] public float LeanMaxPositionDistanceX = 0.5f;
    [Export] public float LeanMaxRotateDistanceZ = 0.25f;
    [Export] public float LeanPositionTweenTime = 0.5f;
    [Export] public float LeanRaycastsTestLength = 0.8f;
    [Export] public float LeanMinCameraDistanceFromWall = 0.3f;
    [Export] public bool LeanMultiRaycastDetect = true;
    [Export] public float LeanMultiRaycastSteps = 0.15f;

    [ExportGroupAttribute("Crouching Settings")]
    [Export] public Array<AudioStream> CrouchingSounds;
    [Export] public int CrouchingAudioDelayMS = 100;
    [Export] public float CrouchingAudioPitch = 0.65f;
    [Export] public float CrouchingAudioPitchRandomOffset = 0.05f;  // for random pitch offset 
    [Export] public float CrouchingVolumeDB = -5f;
    [Export] public Array<AudioStream> UncrouchingSounds;
    [Export] public int UncrouchingAudioDelayMS = 100;
    [Export] public float UncrouchingAudioPitch = 0.5f;
    [Export] public float UncrouchingAudioPitchRandomOffset = 0.05f;  // for random pitch offset 
    [Export] public float UncrouchingVolumeDB = -5f;

    private Vector3 _LastHalfFootStepPosition = Vector3.Zero;
    private int lastIDFootstepSound = -1;

    private bool FootstepNow = false;
    private bool FootstepRight = false;

    Godot.Timer landing_timer = null;
    private float lerpHeadLandY = 0.0f;
    private float lerpHeadLandRotX = 0.0f;

    public all_material_surfaces AllMaterialSurfaces = null;
    public override void _Ready()
    {
        base._Ready();

        // nacteni vsech dat material surfaces
        AllMaterialSurfaces = 
            (all_material_surfaces)GD.Load("res://player/material_surface/all_material_surfaces.tres");

        AudioStreamPlayerFootsteps = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Footsteps");
        AudioStreamPlayerJumpLand = GetNode<AudioStreamPlayer>("AudioStreamPlayer_JumpLand");
        AudioStreamPlayerCrouching = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Crouching");

        // Create timer for landing effect
        landing_timer = new Godot.Timer();
        var callable_FisnishLandingEffect = new Callable(this, "FinishLandingEffect");
        landing_timer.Connect("timeout", callable_FisnishLandingEffect);
        landing_timer.WaitTime = 0.3;
        landing_timer.OneShot = true;
        AddChild(landing_timer);
    }

    public void UpdateInputsProcess(double delta)
    {
        if (!IsInputEnable()) return;
        // hrac pozaduje lean ?

        if (Input.IsActionPressed("leanLeft") && !Input.IsActionPressed("leanRight"))
            objectCamera.SetActualLean(ObjectCamera.ELeanType.Left);
        else if (Input.IsActionPressed("leanRight") && !Input.IsActionPressed("leanLeft"))
            objectCamera.SetActualLean(ObjectCamera.ELeanType.Right);
        else
            objectCamera.SetActualLean(ObjectCamera.ELeanType.Center);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GameMaster.GM.GetIsQuitting()) return;

        base._PhysicsProcess(delta);

        UpdateInputsProcess((float)delta);

        // SET CUSTOM LABEL MOVESPEED AND POSITION OF PLAYER
        float a = Mathf.Snapped(ActualMovementSpeed, 0.1f);
        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(0, this, "MoveSpeed: " + a);
        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(1, this, "Position: " + GlobalPosition);

        CalculateFootSteps((float)delta);
        UpdateLandingHeadBobbing((float)delta);
    }

    public override void EventLanding()
    {
        base.EventLanding();
    }

    public void FinishLandingEffect()
    {
        //GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "finish landing effect");
        lerpHeadLandY = 0.0f;
        lerpHeadLandRotX = 0.0f;
    }

    public override void EventJumping()
    {
        base.EventJumping();

        RandomNumberGenerator a = new RandomNumberGenerator();
        a.Randomize();
        float PitchScale = a.RandfRange(JumpingAudioPitch - (JumpingAudioPitchOffset / 2),
                JumpingAudioPitch + (JumpingAudioPitchOffset / 2));

        // play sounds
        UniversalFunctions.PlayRandomSound(AudioStreamPlayerJumpLand, JumpingSounds, JumpingVolumeDB, PitchScale);

        //Apply camera shake
        GetObjectCamera().ShakeCameraRotation(0.3f, 0.1f, 1.0f, 1.0f);
    }

    private void CalculateFootSteps(float delta)
    {
        InventoryObjectCamera a = GetObjectCamera() as InventoryObjectCamera;

        float halfFootStepLength = FootStepLengthInWalk / 2;
        float lastHalfFootStepDistance = 0.0f;

        if (GetCharacterPosture() == ECharacterPosture.Stand)
        {
            if (GetIsSprint())
                halfFootStepLength = FootStepLengthInSprint / 2;
            else
                halfFootStepLength = FootStepLengthInWalk / 2;
        }
        else
            halfFootStepLength = FootStepLengthInCrouch / 2;

        // pro normal footsteps
        if(IsOnFloor())
            lastHalfFootStepDistance = GlobalPosition.DistanceTo(_LastHalfFootStepPosition);

        // pro first footstep
        if (IsOnFloor() && ActualMovementSpeed <= 5.0f && ActualMovementSpeed > 0.2f &&
            !a.GetHeadBobSystem().GetIsActualStopMovement() && GetIsAnyMoveInputNow())
        {
            _LastHalfFootStepPosition = GlobalPosition;
            halfFootStepLength = 0.02f;
        }

        // je dostatecna vzdalenost pro provedeni footstepu?
        if (lastHalfFootStepDistance >= halfFootStepLength)
        {
            // half footstep change (foot in air - foot on ground)
            FootstepNow = !FootstepNow;
            // if any footstep now ? if false = foot is in air
            if (FootstepNow)
            {
                // change foots (right<->left)
                FootstepRight = !FootstepRight;

                // offset volume db
                float newAddVolumeOffset = 0;
                if (GetCharacterPosture() == ECharacterPosture.Stand)
                {
                    if (GetIsSprint())
                        newAddVolumeOffset = FootstepsVolumeDBInSprint;
                    else
                        newAddVolumeOffset = FootstepsVolumeDBInWalk;
                }
                else
                    newAddVolumeOffset = FootstepsVolumeDBInCrouch;

                // Play Footstep audio
                PlayFootstepSound(newAddVolumeOffset);
            }

            _LastHalfFootStepPosition = GlobalPosition;
            //GD.Print("new footstep");
        }
    }

    public void PlayFootstepSound(float addOffsetVolume = 0.0f,float addOffsetPitch = 0.0f)
    {
        // Detect materal surface name and play specific audio set of footsteps
        all_material_surfaces.EMaterialSurface materialSurface =
            AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());

        //GD.Print(materialSurface);

        if (materialSurface != all_material_surfaces.EMaterialSurface.None)
        {
            // Play random footsteps sound by material surface
            UniversalFunctions.PlayRandomSound(
                AudioStreamPlayerFootsteps,
                AllMaterialSurfaces.GetAudioArray(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep),
                AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep)+addOffsetVolume,
                AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep)+addOffsetPitch);
        }
    }

    public bool GetActualStep() { return FootstepRight; }
    public void SetActualStep(bool newValue){ FootstepRight = newValue; }
    public bool GetFootstepNow() { return FootstepNow; }
    public void SetFootstepNow(bool newValue) { FootstepNow = newValue; }

    private string DetectSurfaceMaterialOfFloor()
    {
        PhysicsDirectSpaceState3D directSpace = GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = GetCharacterLegPosition();
        rayParam.To = GetCharacterLegPosition() + (Vector3.Down * 1);

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
        {
            Node HitCollider = (Node)rayResult["collider"];
            if (HitCollider == null) return "none";

            if (HitCollider.IsInGroup("material_surface_metal"))
                return "material_surface_metal";

            if (HitCollider.IsInGroup("material_surface_wood"))
                return "material_surface_wood";
        }

        return "none";
    }

    private void UpdateLandingHeadBobbing(float delta)
    {
        // Lerp pro landing pos
        HeadGimbalB.Position = HeadGimbalB.Position.Lerp(
            new Vector3(0, lerpHeadLandY, 0), lerpLandingSpeedModifier * delta);


        // Lerp pro landing rot
        if (objectCamera == null) return;

        objectCamera.GimbalLand.Rotation = objectCamera.GimbalLand.Rotation.Lerp(
            new Vector3(lerpHeadLandRotX, 0, 0), lerpLandingSpeedModifier * delta);
    }

    // EVENT from basic movement character
    public override void EventLandingEffect(float heightfall)
    {
        base.EventLandingEffect(heightfall);

        float lerpHeight = -0.4f;
        float lerpRot = -0.1f;
        float speedmod = 3.0f;
        
        //Apply camera shake
        GetObjectCamera().ShakeCameraRotation(0.2f,0.2f,1.0f,1.0f);

        if (heightfall < 0.15f)
        {
            
            // very mini
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(very mini) noticable land effect");

            // Detect materal surface name and play specific audio set of footsteps
            EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());
            if (materialSurface != EMaterialSurface.None)
            {
                // Play random sound
                UniversalFunctions.PlayRandomSound(
                    AudioStreamPlayerJumpLand,
                    AllMaterialSurfaces.GetAudioArray(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing)-8,
                    AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing) -0.1f);
            };

            lerpHeight = -0.1f;
            lerpRot = -0.025f;
            
        }
        else if (heightfall <= MiniHeightForLandingEffect)
        {
            // mini land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(mini land)" + "height from start falling: " + heightfall + " m");

            // Detect materal surface name and play specific audio set of footsteps
            EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());
            if (materialSurface != EMaterialSurface.None)
            {
                // Play random sound
                UniversalFunctions.PlayRandomSound(
                    AudioStreamPlayerJumpLand,
                    AllMaterialSurfaces.GetAudioArray(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing)-1,
                    AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing));
            };

            lerpHeight = -0.2f;
            lerpRot = -0.05f;
        }
        else if (heightfall <= SmallHeightForLandingEffect)
        {
            // small land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(small land)" + "height from start falling: " + heightfall + " m");

            // Detect materal surface name and play specific audio set of footsteps
            EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());
            if (materialSurface != EMaterialSurface.None)
            {
                // Play random sound
                UniversalFunctions.PlayRandomSound(
                    AudioStreamPlayerJumpLand,
                    AllMaterialSurfaces.GetAudioArray(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing));
            }

            lerpHeight = -0.4f;
            lerpRot = -0.1f;
        }
        else if (heightfall <= MediumHeightForLandingEffect)
        {
            // medium land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(medium land)" + "height from start falling: " + heightfall + " m");

            // Detect materal surface name and play specific audio set of footsteps
            EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());
            if (materialSurface != EMaterialSurface.None)
            {
                // Play random sound
                UniversalFunctions.PlayRandomSound(
                    AudioStreamPlayerJumpLand,
                    AllMaterialSurfaces.GetAudioArray(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing));
            };

            lerpHeight = -0.65f;
            lerpRot = -0.2f;
        }
        else if (heightfall <= HighHeightForLandingEffect)
        {
            // high land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(high land)" + "height from start falling: " + heightfall + " m");

            // Detect materal surface name and play specific audio set of footsteps
            EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());
            if (materialSurface != EMaterialSurface.None)
            {
                // Play random sound
                UniversalFunctions.PlayRandomSound(
                    AudioStreamPlayerJumpLand,
                    AllMaterialSurfaces.GetAudioArray(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing));
            };

            lerpHeight = -0.8f;
            lerpRot = -0.4f;
        }
        else if (heightfall > HighHeightForLandingEffect)
        {
            // death land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(death land)" + "height from start falling: " + heightfall + " m");

            // Detect materal surface name and play specific audio set of footsteps
            EMaterialSurface materialSurface = AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());
            if (materialSurface != EMaterialSurface.None)
            {
                // Play random sound
                UniversalFunctions.PlayRandomSound(
                    AudioStreamPlayerJumpLand,
                    AllMaterialSurfaces.GetAudioArray(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                    AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                        materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing));
            };

            lerpHeight = -1.0f;
            lerpRot = -0.3f;
            speedmod = 10.0f;

            // call event dead
            EventDead(ECharacterReasonDead.FallFromHeight, heightfall + " m");
            lerpHeadLandY = lerpHeight;
            lerpHeadLandRotX = lerpRot;
            lerpLandingSpeedModifier = speedmod;
            return;
        }

        lerpHeadLandY = lerpHeight;
        lerpHeadLandRotX = lerpRot;
        lerpLandingSpeedModifier = speedmod;

        // Start timer to normal
        if(landing_timer != null)
            landing_timer.Start();
    }

    // EVENT Dead
    public override void EventDead(ECharacterReasonDead newReasonDead, string newAdditionalData = "", 
        bool newPrintToConsole = false)
    {
        base.EventDead(newReasonDead, newAdditionalData, newPrintToConsole);

        GameMaster.GM.Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"Your character dead becouse: " +
            newReasonDead + "( " + newAdditionalData + ")");

        switch (newReasonDead)
        {
            case ECharacterReasonDead.NoHealth:
                break;
            case ECharacterReasonDead.FallFromHeight:
                break;
            default:
                break;
        }

        // DisableInputs for character
        SetInputEnable(false);
    }

    // callable when change character posture (crunch,uncrouch=stand)
    public override async void ChangeCharacterPosture(ECharacterPosture newCharacterPosture)
    {
        base.ChangeCharacterPosture(newCharacterPosture);

        switch (newCharacterPosture)
        {
            case ECharacterPosture.Crunch:
                {
                    await PlayCrouchAudio(CrouchingAudioDelayMS);
                    break;
                }
            case ECharacterPosture.Stand:
                {    
                    await PlayUncrouchAudio(UncrouchingAudioDelayMS);
                    break;
                }
        }
    }

    async Task PlayUncrouchAudio(int newDelay)
    {
        if (AudioStreamPlayerCrouching == null) return;
        if (UncrouchingSounds == null) return;
        if (UncrouchingSounds.Count < 1) return;

        RandomNumberGenerator a = new RandomNumberGenerator();
        a.Randomize();

        // potrebny delay
        await Task.Delay(newDelay);
        
        if (UncrouchingSounds.Count > 0)
        {
            float newPitch = a.RandfRange(UncrouchingAudioPitch - (UncrouchingAudioPitchRandomOffset / 2),
                UncrouchingAudioPitch + (UncrouchingAudioPitchRandomOffset / 2));

            UniversalFunctions.PlayRandomSound(AudioStreamPlayerCrouching, UncrouchingSounds, UncrouchingVolumeDB, newPitch);
        }
    }

    async Task PlayCrouchAudio(int newDelay)
    {
        if (AudioStreamPlayerCrouching == null) return;
        if (CrouchingSounds == null) return;
        if (CrouchingSounds.Count < 1) return;

        RandomNumberGenerator a = new RandomNumberGenerator();
        a.Randomize();

        // potrebny delay
        await Task.Delay(newDelay);

        if(CrouchingSounds.Count > 0)
        {
            float newPitch = a.RandfRange(CrouchingAudioPitch - (CrouchingAudioPitchRandomOffset / 2),
                CrouchingAudioPitch + (CrouchingAudioPitchRandomOffset / 2));

            UniversalFunctions.PlayRandomSound(AudioStreamPlayerCrouching, CrouchingSounds, CrouchingVolumeDB, newPitch);
        }
    }

    public override void FreeAll()
    {
        landing_timer.Stop();
        landing_timer.QueueFree();
        base.FreeAll();
        landing_timer.QueueFree();
        QueueFree();
    }
}
