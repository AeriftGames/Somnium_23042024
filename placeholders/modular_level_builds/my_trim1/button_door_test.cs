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

    public override void UseButtonEffect()
    {
        base.UseButtonEffect();

        animPlayer.Play("buttonPressed");
        UniversalFunctions.PlayRandom3DSound(audioPlayer,audioStreams,-3,0.8f);

        // shake
        FPSCharacterAction actionChar = CGameMaster.GM.GetGame().GetFPSCharacterBase() as FPSCharacterAction;
        if (actionChar != null)
            actionChar.GetCharacterCameraShakeComponent().ApplyUserParamShake(0.5f, 30.0f);
    }
}
