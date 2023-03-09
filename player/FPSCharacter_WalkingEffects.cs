using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

/*
 * *** FPSCharacter_WalkingEffects(0.1) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and provide extra walking effects
 * - simulation footsteps
 * - walking headbob lerping effects and sounds of footsteps
 * - landing camera lerp effect
 * - jumping sound effect
 * TODO - fix small amount walking/stop no sound - it is weird some times
 * TODO - fix (change) volume,pitch etc footsteps and velocity bobhead when player moves crouched
 * TODO - fix landing/inAir/falling/onGround detect system
*/
public partial class FPSCharacter_WalkingEffects : FPSCharacter_BasicMoving
{
    AudioStreamPlayer AudioStreamPlayerFootsteps = null;
    AudioStreamPlayer AudioStreamPlayerJumpLand = null;
    AudioStreamPlayer AudioStreamPlayerCrouching = null;

    [ExportGroupAttribute("Footsteps Settings")]
    [Export] public float FootStepLength = 1.25f;
    [Export] public float WalkCameraLerpHeight = 0.12f;
    [Export] public float RunCameraLerpHeight = 0.25f;
    [Export] public float lerpFootstepSpeedModifier = 2.0f;
    [Export] public Array<AudioStream> FootstepSounds;
    [Export] public float FootstepsVolumeDB = -20.0f;

    [ExportGroupAttribute("Landing Settings")]
    [Export] public float LandCameraLerpHeight = -0.4f;
    [Export] public float LandCameraLerpRotation = -0.1f;
    [Export] public float lerpLandingSpeedModifier = 3.0f;

    [Export] public float MiniHeightForLandingEffect = 0.3f;
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
    [Export] public float CrouchingVolumeDB = -5f;

    private Vector3 _LastHalfFootStepPosition = Vector3.Zero;
    private int lastIDFootstepSound = -1;

    private bool FootstepNow = false;
    private bool FootstepRight = false;
    private float lerpHeadWalkY = 0.0f;

    Godot.Timer landing_timer = null;
    private float lerpHeadLandY = 0.0f;
    private float lerpHeadLandRotX = 0.0f;

    // lean

    public override void _Ready()
    {
        base._Ready();

        AudioStreamPlayerFootsteps = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Footsteps");
        AudioStreamPlayerJumpLand = GetNode<AudioStreamPlayer>("AudioStreamPlayer_JumpLand");
        AudioStreamPlayerCrouching = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Crouching");

        // Create timer for landing effect
        landing_timer = new Godot.Timer();
        var callable_FisnishLandingEffect = new Callable(this,"FinishLandingEffect");
        landing_timer.Connect("timeout", callable_FisnishLandingEffect);
        landing_timer.WaitTime = 0.3;
        landing_timer.OneShot = true;
        AddChild(landing_timer);
    }

    public void UpdateInputsProcess(double delta)
    {
        // hrac pozaduje lean ?

        if(Input.IsActionPressed("leanLeft") && !Input.IsActionPressed("leanRight"))
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

        /*
        GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "MoveSpeed: " +
            Mathf.Snapped(ActualMovementSpeed, 0.1f));*/

        CalculateFootSteps((float)delta);

        UpdateWalkHeadBobbing((float)delta);
        UpdateLandingHeadBobbing((float)delta);

        UpdateLeaning((float)delta);
    }

    public override void EventLanding()
    {
        base.EventLanding();
        /*
        PlayRandomSound(AudioStreamPlayerJumpLand, smallHeightLandingSounds, LandingVolumeDB, 0.5f);

        lerpHeadLandY = LandCameraLerpHeight;
        lerpHeadLandRotX = LandCameraLerpRotation;
        landing_timer.Start();
        */
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

        PlayRandomSound(AudioStreamPlayerJumpLand, JumpingSounds, JumpingVolumeDB, 1.0f);
    }

    private void CalculateFootSteps(float delta)
    {
        float halfFootStepLength = FootStepLength / 2;

        float lastHalfFootStepDistance = 0.0f;

        if (IsOnFloor())
            lastHalfFootStepDistance = GlobalPosition.DistanceTo(_LastHalfFootStepPosition);
        if (lastHalfFootStepDistance >= halfFootStepLength)
        {
            // half footstep change (foot in air - foot on ground)
            FootstepNow = !FootstepNow;

            // if any footstep now ? if false = foot is in air
            if (FootstepNow)
            {
                // change foots (right<->left)
                FootstepRight = !FootstepRight;

                // Play footstep audio
                PlayRandomSound(AudioStreamPlayerFootsteps, FootstepSounds, FootstepsVolumeDB, 1.0f);
            }

            _LastHalfFootStepPosition = GlobalPosition;
            //GD.Print("new footstep");
        }
    }

    private void UpdateWalkHeadBobbing(float delta)
    {
        if (FootstepNow)
        {
            // foot touch ground now
            if (_isSprint)
                lerpHeadWalkY = -RunCameraLerpHeight;
            else
                lerpHeadWalkY = -WalkCameraLerpHeight;
        }
        else
        {
            // foot is above to ground
            if (_isSprint)
                lerpHeadWalkY = RunCameraLerpHeight;
            else
                lerpHeadWalkY = WalkCameraLerpHeight;
        }

        // if actualmove is smaller than testing value, centered headlerpY and speedUP lerp to normal 
        if (ActualMovementSpeed <= 0.2f)
        {
            lerpHeadWalkY = 0.0f;
            lerpFootstepSpeedModifier = 3.0f;
        }
        
        // Lerp pro head bobbing walk Y
        HeadGimbalA.Position = HeadGimbalA.Position.Lerp(
            new Vector3(0, lerpHeadWalkY, 0), lerpFootstepSpeedModifier * delta);
    }

    private void UpdateLandingHeadBobbing(float delta)
    {
        // Lerp pro landing pos
        HeadGimbalB.Position = HeadGimbalB.Position.Lerp(
            new Vector3(0, lerpHeadLandY, 0), lerpLandingSpeedModifier * delta);

        
        // Lerp pro landing rot
        objectCamera.GimbalLand.Rotation = objectCamera.GimbalLand.Rotation.Lerp(
            new Vector3(lerpHeadLandRotX, 0, 0), lerpLandingSpeedModifier * delta);
    }

    private void PlayRandomSound(AudioStreamPlayer audioPlayer,Array<AudioStream> audioStreams,float volumeDB, float pitch)
    {
        if (audioPlayer == null) return;
        if (audioStreams.Count < 1) return;

        // random pick sound from array and play it
        RandomNumberGenerator random = new RandomNumberGenerator();
        int id = 0;

        // 20 chances
        for (int i = 0; i < 20; i++)
        {
            // randomize sound id from array
            random.Randomize();
            id = random.RandiRange(0, audioStreams.Count - 1);

            // if is not same, break for loop
            if (audioPlayer.Stream != audioStreams[id])
                break;
        }

        // play sounds
        audioPlayer.VolumeDb = volumeDB;
        audioPlayer.PitchScale = pitch;
        audioPlayer.Stream = audioStreams[id];
        audioPlayer.Play();
    }

    // EVENT from basic movement character
    public override void EventLandingEffect(float heightfall)
    {
        base.EventLandingEffect(heightfall);

        float lerpHeight = -0.4f;
        float lerpRot = -0.1f;
        float speedmod = 3.0f;

        if (heightfall < 0.15f)
        {
            // very mini
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(very mini) noticable land effect");
            PlayRandomSound(AudioStreamPlayerJumpLand, miniHeightLandingSounds, LandingVolumeDB - 8, 0.7f);
            lerpHeight = -0.1f;
            lerpRot = -0.025f;

        }
        else if (heightfall <= MiniHeightForLandingEffect)
        {
            // mini land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(mini land)" + "height from start falling: " + heightfall + " m");
            PlayRandomSound(AudioStreamPlayerJumpLand, miniHeightLandingSounds, LandingVolumeDB - 1, 0.5f);
            lerpHeight = -0.2f;
            lerpRot = -0.05f;   
        }
        else if (heightfall <= SmallHeightForLandingEffect)
        {
            // small land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(small land)" + "height from start falling: " + heightfall + " m");
            PlayRandomSound(AudioStreamPlayerJumpLand, smallHeightLandingSounds, LandingVolumeDB, 0.5f);
            lerpHeight = -0.4f;
            lerpRot = -0.1f;
        }
        else if (heightfall <= MediumHeightForLandingEffect)
        {
            // medium land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(medium land)" + "height from start falling: " + heightfall + " m");
            PlayRandomSound(AudioStreamPlayerJumpLand, mediumHeightLandingSounds, LandingVolumeDB, 0.5f);
            lerpHeight = -0.65f;
            lerpRot = -0.2f;
        }
        else if (heightfall <= HighHeightForLandingEffect)
        {
            // high land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(high land)" + "height from start falling: " + heightfall + " m");
            PlayRandomSound(AudioStreamPlayerJumpLand, mediumHeightLandingSounds, LandingVolumeDB+3, 0.6f);
            lerpHeight = -0.8f;
            lerpRot = -0.4f;
        }
        else if (heightfall > HighHeightForLandingEffect)
        {
            // death land
            GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "(death land)" + "height from start falling: " + heightfall + " m");
            PlayRandomSound(AudioStreamPlayerJumpLand, deathHeightLandingSounds, LandingVolumeDB, 0.5f);
            lerpHeight = -1.0f;
            lerpRot = -0.3f;
            speedmod = 10.0f;

            // call event dead
            EventDead("fall from height: " + heightfall + " m");
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
    public override void EventDead(string reasonDead)
    {
        base.EventDead(reasonDead);

        GameMaster.GM.Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"Your character dead becouse: " + reasonDead);

        // DisableInputs for character
        SetInputEnable(false);
    }

    public void UpdateLeaning(double delta)
    {

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

                    break;
                }
        }
    }

    async Task PlayCrouchAudio(int newDelay)
    {
        // potrebny delay
        await Task.Delay(newDelay);
        AudioStreamPlayerCrouching.Play();
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
