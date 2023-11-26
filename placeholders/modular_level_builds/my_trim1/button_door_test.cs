using Godot;
using Godot.Collections;
using System;
using System.Security.AccessControl;

public partial class button_door_test : BasicButtonObject
{
    MeshInstance3D meshButton;
    AnimationPlayer animPlayer;
    AudioStreamPlayer3D audioPlayer;

    [Export] Array<AudioStream> audioStreams;


    public override void _Ready()
	{
        base._Ready();

        meshButton = GetNode<MeshInstance3D>("button");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void UseButtonEffect()
    {
        base.UseButtonEffect();

        animPlayer.Play("buttonPressed");
        UniversalFunctions.PlayRandom3DSound(audioPlayer,audioStreams,-3,0.8f);

        FPSCharacter_Inventory character = CGameMaster.GM.GetGame().GetFPSCharacterOld() as FPSCharacter_Inventory;
        character.GetObjectCamera().ShakeCameraRotation(0.2f,0.1f,2,2);
    }
}
