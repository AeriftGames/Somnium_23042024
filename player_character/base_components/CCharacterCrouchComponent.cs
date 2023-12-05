using Godot;
using System;
using System.Threading.Tasks;

public partial class CCharacterCrouchComponent : CBaseComponent
{
    
    [Export] public bool TOGGLE_CROUCH = false;
    [Export(PropertyHint.Range, "1,5,0.1")] public float CROUCH_SPEED = 2.0f;
    [Export] public AudioStream AudioStreamCrouch;
    [Export] public AudioStream AudioStreamUnCrouch;

    private AnimationPlayer animPlayerCrouch;
    private ShapeCast3D shapeCastUncrouch;
    private AudioStreamPlayer audioStreamPlayerCrouch;

    private bool isCrouching = false;

    public override void _Ready()
	{
        base._Ready();

        animPlayerCrouch = GetNode<AnimationPlayer>("AnimationPlayerCrouch");
        shapeCastUncrouch = GetNode<ShapeCast3D>("ShapeCastUncrouch");
        audioStreamPlayerCrouch = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Crouch");
    }

    public override void PostInit(FpsCharacterBase newOurCharacter)
    {
        base.PostInit(newOurCharacter);

        shapeCastUncrouch.AddException(ourCharacterBase);
    }

    public void CheckAndApplyCrouch(StringName newInput)
    {
        if (ourCharacterBase.GetCharacterMovementComponent() == null) return;
        bool isOnFloor = ourCharacterBase.GetCharacterMovementComponent().GetIsOnFloor();

        if (TOGGLE_CROUCH == true)
        {
            if (Input.IsActionJustPressed(newInput) && isOnFloor)
                ToggleCrouch();
        }
        else if(TOGGLE_CROUCH == false)
        {
            if (Input.IsActionJustPressed(newInput) && isOnFloor)
                SetCrouch(true);
            else if (Input.IsActionJustReleased(newInput) || isOnFloor == false)
                if(isCrouching == true)
                    SetCrouch(false);
        }
    }

    private void ToggleCrouch(){ SetCrouch(!isCrouching); }
    private void SetCrouch(bool newCrouch)
    {
        if(TOGGLE_CROUCH == true)
        {
            if (newCrouch && animPlayerCrouch.IsPlaying() == false)
            {
                animPlayerCrouch.Play("Crouch", -1, CROUCH_SPEED);

                audioStreamPlayerCrouch.Stream = AudioStreamCrouch;
                audioStreamPlayerCrouch.Play();
            }
            else if (shapeCastUncrouch.IsColliding() == false && animPlayerCrouch.IsPlaying() == false)
            {
                animPlayerCrouch.Play("Crouch", -1, -CROUCH_SPEED, true);

                audioStreamPlayerCrouch.Stream = AudioStreamUnCrouch;
                audioStreamPlayerCrouch.Play();
            }
        }
        else if(TOGGLE_CROUCH == false)
        {
            if (newCrouch == true && shapeCastUncrouch.IsColliding() == false)
            {
                animPlayerCrouch.Play("Crouch", -1, CROUCH_SPEED);
                isCrouching = true;

                audioStreamPlayerCrouch.Stream = AudioStreamCrouch;
                audioStreamPlayerCrouch.Play();
            }
            else if (newCrouch == false && shapeCastUncrouch.IsColliding() == true)
            {
                Uncrouch_Check();
            }
            else if (newCrouch == false && shapeCastUncrouch.IsColliding() == false)
            {
                animPlayerCrouch.Play("Crouch", -1, -CROUCH_SPEED, true);
                isCrouching = false;

                audioStreamPlayerCrouch.Stream = AudioStreamUnCrouch;
                audioStreamPlayerCrouch.Play();
            }
        }
    }

    public void _on_animation_player_crouch_animation_started(StringName animName)
    {
        if(animName == "Crouch" && TOGGLE_CROUCH == true)
            isCrouching = !isCrouching;
    }

    private async void Uncrouch_Check()
    {
        if (shapeCastUncrouch.IsColliding() == false)
        {
            SetCrouch(false);
        }
        else if (shapeCastUncrouch.IsColliding() == true)
        {
            await Task.Delay(100);
            Uncrouch_Check();
        }
    }

    public bool GetIsCrouched() {  return isCrouching; }

}
