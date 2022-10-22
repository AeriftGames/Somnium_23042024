using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Threading;

/*
 * *** FPSCharacter_WalkingEffects(0.1) ***
 * 
 * - this class is inheret from FPSCharacter_WalkingEffects and provide extra walking effects
 * - simulation footsteps
 * - walking headbob lerping effects and sounds of footsteps
 * - landing camera lerp effect
 * - jumping sound effect
 * TODO - fix landing sound precision play time - OK
 * TODO - fix small amount walking/stop no sound - it is weird some times
 * TODO - fix (change) volume,pitch etc footsteps and velocity bobhead when player moves crouched
 * TODO - fix landing/inAir/falling/onGround detect system
*/
public partial class FPSCharacter_WalkingEffects : FPSCharacter_BasicMoving
{
    AudioStreamPlayer AudioStreamPlayerFootsteps = null;
    AudioStreamPlayer AudioStreamPlayerJumpLand = null;

    [ExportGroupAttribute("Footsteps Settings")]
    [Export] public float FootStepLength = 1.5f;
    [Export] public float WalkCameraLerpHeight = 0.25f;
    [Export] public float RunCameraLerpHeight = 0.25f;
    [Export] public float lerpFootstepSpeedModifier = 1.0f;

    [ExportGroupAttribute("Landing Settings")]
    [Export] public float LandCameraLerpHeight = -0.4f;
    [Export] public float LandCameraLerpRotation = -0.1f;
    [Export] public float lerpLandingSpeedModifier = 3.0f;

    [ExportGroupAttribute("Audio Settings")]
    [Export] public AudioStream[] FootstepSounds;
    [Export] public AudioStream[] JumpingSounds;
    [Export] public AudioStream[] LandingSounds;

    private float _ActualMovementSpeed = 0.0f;
    private Vector3 _LastPosition = Vector3.Zero;
    private Vector3 _LastHalfFootStepPosition = Vector3.Zero;
    private int lastIDFootstepSound = -1;

    private bool FootstepNow = false;
    private bool FootstepRight = false;
    private float lerpHeadWalkY = 0.0f;

    Godot.Timer landing_timer = new Godot.Timer();
    private float lerpHeadLandY = 0.0f;
    private float lerpHeadLandRotX = 0.0f;

    public override void _Ready()
    {
        base._Ready();

        AudioStreamPlayerFootsteps = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Footsteps");
        AudioStreamPlayerJumpLand = GetNode<AudioStreamPlayer>("AudioStreamPlayer_JumpLand");

        // Create timer for landing effect
        var callable_FisnishLandingEffect = new Callable(FinishLandingEffect);
        landing_timer.Connect("timeout", callable_FisnishLandingEffect);
        landing_timer.WaitTime = 0.3;
        landing_timer.OneShot = true;
        AddChild(landing_timer);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        CalculateFootSteps((float)delta);
        UpdateWalkHeadBobbing((float)delta);
        UpdateLandingHeadBobbing((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void Landing()
    {
        base.Landing();

        PlayRandomSound(AudioStreamPlayerJumpLand, LandingSounds, 0.5f);

        lerpHeadLandY = LandCameraLerpHeight;
        lerpHeadLandRotX = LandCameraLerpRotation;
        landing_timer.Start();

    }

    public void FinishLandingEffect()
    {
        GD.Print("finish landing");
        lerpHeadLandY = 0.0f;
        lerpHeadLandRotX = 0.0f;
    }

    public override void Jumping()
    {
        base.Jumping();

        PlayRandomSound(AudioStreamPlayerJumpLand, JumpingSounds, 1.0f);
    }

    private void CalculateFootSteps(float delta)
    {
        float halfFootStepLength = FootStepLength / 2;

        float lastHalfFootStepDistance = 0.0f;

        // Calculate actual movement speed
        _ActualMovementSpeed = GlobalPosition.DistanceTo(_LastPosition) * 40000.0f * delta;

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
                PlayRandomSound(AudioStreamPlayerFootsteps, FootstepSounds, 1.0f);
            }

            _LastHalfFootStepPosition = GlobalPosition;
            //GD.Print("new footstep");
        }

        //Update LastPosition with actual position
        _LastPosition = GlobalPosition;
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
        if (_ActualMovementSpeed <= 0.2f)
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
        HeadGimbalB.Position = HeadGimbalB.Position.Lerp(
            new Vector3(0, lerpHeadLandY, 0), lerpLandingSpeedModifier * delta);

        HeadGimbalB.Rotation = HeadGimbalB.Rotation.Lerp(
            new Vector3(lerpHeadLandRotX, 0, 0), lerpLandingSpeedModifier * delta);
    }

    private void PlayRandomSound(AudioStreamPlayer audioPlayer, AudioStream[] audioStreams, float pitch)
    {
        if (audioPlayer == null) return;
        if (audioStreams.Length < 1) return;

        // random pick sound from array and play it
        RandomNumberGenerator random = new RandomNumberGenerator();
        int id = 0;

        // 20 chances
        for (int i = 0; i < 20; i++)
        {
            // randomize sound id from array
            random.Randomize();
            id = random.RandiRange(0, audioStreams.Length - 1);

            // if is not same, break for loop
            if (audioPlayer.Stream != audioStreams[id])
                break;
        }

        // play sounds
        audioPlayer.PitchScale = pitch;
        audioPlayer.Stream = audioStreams[id];
        audioPlayer.Play();
    }
}
