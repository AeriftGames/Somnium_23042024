using Godot;
using System;

public partial class Flashlight_Item : SpotLight3D
{
    [Export] public bool EnableOnStart = false;
	[Export] public AudioStream AudioStream_TurnOn;
    [Export] public AudioStream AudioStream_TurnOff;
    [Export] public float TurnOn_Pitch = 1.0f;
    [Export] public float TurnOn_VolumeDb = -10.0f;
    [Export] public float TurnOff_Pitch = 0.8f;
    [Export] public float TurnOff_VolumeDb = -10.0f;


    private AnimationPlayer AnimPlayer = null;
	private AudioStreamPlayer AudioPlayer = null;

    private bool isEnable = false;
	public override void _Ready()
	{
        base._Ready();

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AudioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        // prepne pocatecni stav bez zvuku
        EnableFlashlight(EnableOnStart, true);
    }

	public override void _Process(double delta)
	{

	}

    public void ShowMesh( bool newEnable ) { GetNode<MeshInstance3D>("SM_Flashlight").Visible = newEnable; }

    public void UseAction() { EnableFlashlight(!isEnable); }

    public void EnableFlashlight(bool newEnable, bool newWithoutAudio = false)
    {
        isEnable = newEnable;

        if (isEnable)
        {
            if(newWithoutAudio == false)
                PlaySound(true);

            AnimPlayer.Play("TurnOn");
        }
        else
        {
            if(newWithoutAudio == false)
                PlaySound(false);

            AnimPlayer.Play("TurnOff");
        }
    }

    public void PlaySound(bool newEnable)
    {
        GD.Print(AudioPlayer);

        if (newEnable)
        {
            //ON
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO, "Flaslight ON");

            // Audio play
            AudioPlayer.VolumeDb = TurnOn_VolumeDb;
            AudioPlayer.PitchScale = TurnOn_Pitch;
            AudioPlayer.Stream = AudioStream_TurnOn;
            AudioPlayer.Play();
        }
        else
        {
            //OFF
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO, "Flaslight OFF");

            // Audio play
            AudioPlayer.VolumeDb = TurnOff_VolumeDb;
            AudioPlayer.PitchScale = TurnOff_Pitch;
            AudioPlayer.Stream = AudioStream_TurnOff;
            AudioPlayer.Play();
        }
    }
}

