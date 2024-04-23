using Godot;
using System;

public partial class LaserDoor : Node3D
{
	AnimationPlayer player;
	AudioStreamPlayer3D audioPlayer;

	bool isEnable = false;
	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer");
		audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("test_door"))
			ToggleEnableLaser();
	}

	public void ToggleEnableLaser() { EnableLaser(!isEnable); }

	public void EnableLaser(bool newEnable)
	{
		if (newEnable)
		{
			isEnable = true;
			player.Play("LaserOn");
		}
		else
		{
			isEnable = false;
            player.Play("LaserOff");
        }
	}

	public void PlayEnableSoundNow(bool newEnable)
	{
		if (newEnable)
			audioPlayer.Play();
		else
            audioPlayer.Play();
    }
}
