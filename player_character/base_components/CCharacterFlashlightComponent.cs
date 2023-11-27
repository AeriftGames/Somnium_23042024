using Godot;
using System;
using System.Runtime.Remoting;

public partial class CCharacterFlashlightComponent : Node3D
{
	[Export] public AudioStream AudioFlashlight_On;
	[Export] public AudioStream AudioFlashlight_Off;
    [Export] public float AudioFlashlight_On_Pitch = 1.0f;
    [Export] public float AudioFlashlight_On_VolumeDb = -10.0f;
    [Export] public float AudioFlashlight_Off_Pitch = 0.8f;
    [Export] public float AudioFlashlight_Off_VolumeDb = -10.0f;

    private FpsCharacterBase ourCharacter = null;
	private SpotLight3D Flashlight = null;
	private AudioStreamPlayer AudioStreamPlayer_Flashlight = null;

	private Vector3 workingLookAt = Vector3.Zero;
	private bool isEnable = false;

	public void PostInit(FpsCharacterBase newCharacterBase)
	{
		ourCharacter = newCharacterBase;

		Flashlight = GetNode<SpotLight3D>("Flashlight");
		AudioStreamPlayer_Flashlight = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Flashlight");

		Flashlight.Visible = false;
	}

	public override void _Process(double delta)
	{
		// INPUT
		if (Input.IsActionJustPressed("ToggleFlashlight"))
			EnableFlashlight(!isEnable);

		// UPDATE
		UpdateByCameraLook(delta);
	}

	public void EnableFlashlight(bool newEnable)
	{
		isEnable = newEnable;

		if (isEnable)
		{
			Flashlight.Visible = true;
			PlaySound(true);

        }
		else
		{
			Flashlight.Visible = false;
            PlaySound(false);

        }
	}

	private void UpdateByCameraLook(double delta)
	{
		// SET GLOBAL POS
		Flashlight.GlobalPosition = ourCharacter.GetCharacterLookComponent().GetMainCamera().
			GlobalPosition + ourCharacter.GetCharacterLookComponent().GetMainCamera().
			GlobalPosition.DirectionTo(ourCharacter.GetCharacterLookComponent().GetMainCameraLookingPoint()) * 0.2f;

		// LERP LOOKINGPOINT FOR LOOKING
		workingLookAt = workingLookAt.Lerp(ourCharacter.GetCharacterLookComponent().GetMainCameraLookingPoint(),
			(float)delta * 50.0f);

		// LOOK AT INTERPOLATE POINT
		Flashlight.LookAt(workingLookAt);
	}

	public void PlaySound(bool newEnable)
	{
        if (newEnable)
        {
            //ON
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO, "Flaslight ON");

            // Audio play
            AudioStreamPlayer_Flashlight.VolumeDb = AudioFlashlight_On_VolumeDb;
            AudioStreamPlayer_Flashlight.PitchScale = AudioFlashlight_On_Pitch;
            AudioStreamPlayer_Flashlight.Stream = AudioFlashlight_On;
            AudioStreamPlayer_Flashlight.Play();
        }
        else
        {
            //OFF
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO, "Flaslight OFF");

            // Audio play
            AudioStreamPlayer_Flashlight.VolumeDb = AudioFlashlight_Off_VolumeDb;
            AudioStreamPlayer_Flashlight.PitchScale = AudioFlashlight_Off_Pitch;
            AudioStreamPlayer_Flashlight.Stream = AudioFlashlight_Off;
            AudioStreamPlayer_Flashlight.Play();
        }
    }
}
