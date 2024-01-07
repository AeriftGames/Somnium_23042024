using Godot;
using System;

public partial class CCharacterHeadBobComponent : CBaseComponent
{
    private Node3D HeadBobNode;
    private Node3D CameraSway;

    AudioStreamPlayer AudioStreamPlayerFootsteps = null;

    [Export] public bool EnableEffectPos = true;
    [Export] public bool EnableEffectRot = true;
    [Export] public bool EnableEffectSway = true;

    [ExportGroupAttribute("Sway Settings")]
    [Export] public float SwayAmmount = 0.75f;
    [Export] public float SwayLerpSpeed = 2.5f;

    [ExportGroupAttribute("Footsteps Settings")]
    [Export] public float FootStepLengthInWalk = 1.25f;
    [Export] public float FootStepLengthInSprint = 1.28f;
    [Export] public float FootStepLengthInCrouch = 0.95f;

    [Export] public float FootstepsVolumeDBInWalk = -2.0f;
    [Export] public float FootstepsVolumeDBInSprint = 1.0f;
    [Export] public float FootstepsVolumeDBInCrouch = -7.0f;
    [Export] public float FootstepsAudioPitch = 0.75f;

    [ExportGroupAttribute("HeadBobMovingPos")]
    [Export] public float headBobbingWalkValue = 0.2f;
    [Export] public float headBobbingSprintValue = 0.3f;
    [Export] public float headBobbingCrouchValue = 0.15f;
    [Export] public float headBobbingDeltaToMove = 1.0f;
    [Export] public float headBobbingDeltaToStop = 3.0f;
    [ExportGroupAttribute("HeadBobMovingRot")]
    [Export] public float headBobRotDegWalkValue = 1.5f;
    [Export] public float headBobRotDegSprintValue = 3.0f;
    [Export] public float headBobRotDegCrouchValue = 1.0f;
    [Export] public float headBobRotDeltaToMove = 1.0f;
    [Export] public float headBobRotDeltaToStop = 3.0f;
    [ExportGroupAttribute("Other")]
    [Export] public float WalkCameraLerpHeight = 0.12f;
    [Export] public float RunCameraLerpHeight = 0.2f;
    [Export] public float CrouchCameraLerpHeight = 0.1f;
    [Export] public float headBobMovingXDelta = 1.0f;
    [Export] public float headBobMovingYDelta = 2.0f;

    // for moving bob
    float headBobMovingX = 0.0f;
    float headBobRot = 0.0f;
    float headBobRotDelta = 1.0f;
    int lastFootState = 0;

    float lerpHeadWalkY = 0;
    int actStepsToEffect = 0;
    int numStepsToEffect = 1;

    private bool isActualStopMovement = false;
    private bool FootstepNow = false;
    private bool FootstepRight = false;

    private Vector3 _LastHalfFootStepPosition = Vector3.Zero;
    private int lastIDFootstepSound = -1;

    private Vector3 workRot = Vector3.Zero;

    private all_material_surfaces AllMaterialSurfaces = null;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        if (EnableComponent == false) { return; }

        base.PostInit(newCharacterBase);

        AudioStreamPlayerFootsteps = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Footsteps");

        HeadBobNode = GetNode<Node3D>("%HeadBob");
        CameraSway = ourCharacterBase.GetCharacterLookComponent().GetCameraSway();

        // nacteni vsech dat material surfaces
        AllMaterialSurfaces =
            (all_material_surfaces)GD.Load("res://player/material_surface/all_material_surfaces.tres");
    }
    public void Update(double delta)
    {
        if(EnableComponent == false ) { return; }
        
        //CalculateFootSteps((float)delta);
        //UpdateWalkHeadBobbing((float)delta);
        /*
        if (EnableEffectPos)
        {
            Vector3 pos = HeadBobNode.Position;
            pos.X = Mathf.Lerp(pos.X, headBobMovingX, headBobMovingXDelta * (float)delta);
            pos.Y = Mathf.Lerp(pos.Y, lerpHeadWalkY, headBobMovingYDelta * (float)delta);
            HeadBobNode.Position = pos;
        }
        
        if (EnableEffectRot)
        {
            HeadBobNode.RotationDegrees = 
                HeadBobNode.RotationDegrees.Lerp(new Vector3(0, 0, headBobRot),
                (float)delta * headBobRotDelta);
        }
        */
        if (EnableEffectSway)
        {
            Vector3 workDir = new Vector3(ourCharacterBase.GetCharacterMovementComponent().GetInputDir().Normalized().Y * 
                ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed(),
                0.0f, -ourCharacterBase.GetCharacterMovementComponent().GetInputDir().Normalized().X * 
                ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed());

            workRot = workRot.Lerp(workDir.Normalized()*(0.05f*SwayAmmount), (float)delta *2.5f);
            workRot.Y = 0;

            CameraSway.Rotation = new Vector3(workRot.X,CameraSway.Rotation.Y,workRot.Z);
        }
    }

    public void UpdateWalkHeadBobbing(int actualStepState, float delta)
    {
        if (actualStepState == lastFootState) return;

        if (actualStepState == 0)
        {
            // noha ve vzduchu
            headBobMovingX = 0.0f;
            headBobMovingXDelta = headBobbingDeltaToStop;
            headBobRot = 0.0f;
            headBobRotDelta = headBobRotDeltaToStop;
        }
        else if (actualStepState == 1)
        {
            if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
            {
                if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
                {
                    //sprint
                    headBobMovingX = headBobbingSprintValue;
                    headBobRot = headBobRotDegSprintValue;
                }
                else
                {
                    //walk
                    headBobMovingX = headBobbingWalkValue;
                    headBobRot = headBobRotDegWalkValue;
                }
            }
            else
            {
                //crouch
                headBobMovingX = headBobbingCrouchValue;
                headBobRot = headBobRotDegCrouchValue;
            }

            headBobMovingXDelta = headBobbingDeltaToMove;
            headBobRotDelta = headBobRotDeltaToMove;
        }
        else if (actualStepState == 2)
        {
            if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
            {
                if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
                {
                    //sprint
                    headBobMovingX = -headBobbingSprintValue;
                    headBobRot = -headBobRotDegSprintValue;
                }
                else
                {
                    //walk
                    headBobMovingX = -headBobbingWalkValue;
                    headBobRot = -headBobRotDegWalkValue;
                }
            }
            else
            {
                //crouch
                headBobMovingX = -headBobbingCrouchValue;
                headBobRot = -headBobRotDegCrouchValue;
            }

            headBobMovingXDelta = headBobbingDeltaToMove;
            headBobRotDelta = headBobRotDeltaToMove;
        }

        lastFootState = actualStepState;
    }
    private void UpdateWalkHeadBobbing(float delta)
    {
        if (GetFootstepNow())
        {
            // foot touch ground now
            if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
            {
                if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
                    lerpHeadWalkY = -RunCameraLerpHeight;
                else
                    lerpHeadWalkY = -WalkCameraLerpHeight;
            }
            else
                lerpHeadWalkY = -CrouchCameraLerpHeight;

            if (actStepsToEffect == 0)
            {
                if (GetActualStep() == true)
                    UpdateWalkHeadBobbing(1, delta);
                else
                    UpdateWalkHeadBobbing(2, delta);
            }

            actStepsToEffect++;
        }
        else
        {
            // noha above on ground
            if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
            {
                if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
                    lerpHeadWalkY = RunCameraLerpHeight;
                else
                    lerpHeadWalkY = WalkCameraLerpHeight;
            }
            else
                lerpHeadWalkY = CrouchCameraLerpHeight;
        }

        if (actStepsToEffect >= numStepsToEffect)
            actStepsToEffect = 0;

        // if actualmove is smaller than testing value, centered headlerpY and speedUP lerp to normal 
        if (ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() <= 1.0f)
        {
            // dodatecny zvuk kroku pri zastaveni, vyresetovani aktualniho footstepu na opacny footstep
            if (!isActualStopMovement)
            {
                PlayFootstepSound(-5.0f, -0.1f);

                isActualStopMovement = true;
                actStepsToEffect = 0;
                SetFootstepNow(true);
                SetActualStep(!GetActualStep());
            }

            lerpHeadWalkY = 0.0f;
            headBobMovingXDelta = headBobbingDeltaToStop;
            //
            UpdateWalkHeadBobbing(0, delta);
        }
        else
        {
            isActualStopMovement = false;
        }
    }
    private void CalculateFootSteps(float delta)
    {
        float halfFootStepLength = FootStepLengthInWalk / 2;
        float lastHalfFootStepDistance = 0.0f;

        if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
        {
            if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
                halfFootStepLength = FootStepLengthInSprint / 2;
            else
                halfFootStepLength = FootStepLengthInWalk / 2;
        }
        else
            halfFootStepLength = FootStepLengthInCrouch / 2;

        // pro normal footsteps
        if (ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor())
            lastHalfFootStepDistance = ourCharacterBase.GlobalPosition.DistanceTo(_LastHalfFootStepPosition);
        /*
        // pro first footstep
        if (ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor() &&
            ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() <= 5.0f &&
            ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 0.2f &&
            !GetIsActualStopMovement() && 
            ourCharacterBase.GetCharacterMovementComponent().GetInputDir() != Vector2.Zero)
        {
            _LastHalfFootStepPosition = ourCharacterBase.GlobalPosition;
            halfFootStepLength = 0.02f;
        }*/
        
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
                if (ourCharacterBase.GetCharacterCrouchComponent().GetIsCrouched() == false)
                {
                    if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "RunningPlayerState")
                        newAddVolumeOffset = FootstepsVolumeDBInSprint;
                    else
                        newAddVolumeOffset = FootstepsVolumeDBInWalk;
                }
                else
                    newAddVolumeOffset = FootstepsVolumeDBInCrouch;

                // Play Footstep audio
                PlayFootstepSound(newAddVolumeOffset, FootstepsAudioPitch);
            }

            _LastHalfFootStepPosition = ourCharacterBase.GlobalPosition;
            //GD.Print("new footstep");
        }
    }

    public bool GetIsActualStopMovement() { return isActualStopMovement; }
    public bool GetActualStep() { return FootstepRight; }
    public void SetActualStep(bool newValue) { FootstepRight = newValue; }
    public bool GetFootstepNow() { return FootstepNow; }
    public void SetFootstepNow(bool newValue) { FootstepNow = newValue; }
    public async void PlayFootstepSound(float addOffsetVolume = 0.0f, float addOffsetPitch = 0.0f)
    {
        await ToSignal(GetTree(),SceneTree.SignalName.PhysicsFrame);

        // Detect materal surface name and play specific audio set of footsteps
        all_material_surfaces.EMaterialSurface materialSurface =
            AllMaterialSurfaces.GetMaterialSurfaceFromGroup(UniversalFunctions.DetectSurfaceMaterialOfFloor(
                ourCharacterBase,ourCharacterBase.GlobalPosition+(Vector3.Up*0.3f)));

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