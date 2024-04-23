using Godot;
using System;
using System.Runtime.InteropServices;

public partial class CHealthAudioComponent : Node
{
    [Export] public Godot.Collections.Array<AudioStream> HealthAudioStreams;
    [Export] public float VolumeDB = 0.6f;
    [Export] public float Pitch = 1.0f;

    private FPSCharacterAction ourActionCharacter = null;
    private AudioStreamPlayer AudioHealthPlayer = null;


    public void PostInit(FPSCharacterAction newFPSCharacterAction)
    {
        ourActionCharacter = newFPSCharacterAction;
        AudioHealthPlayer = GetNode<AudioStreamPlayer>("%AudioStreamPlayer_Health");
    }

    public void PlayHurtAudio(float newDamage)
    {
        if (AudioHealthPlayer == null || HealthAudioStreams.Count == 0) return;

        UniversalFunctions.PlayRandomSound(AudioHealthPlayer, HealthAudioStreams, VolumeDB, Pitch);
    }
}
