using Godot;
using Godot.Collections;
using System;

/*
 * vylepsit system detekce = aby se dvere zacaly otevirat/zavirat az kdyz se splni aktualni animace
 */

[Tool]
public partial class door_rounded_test : Node3D
{
    public enum EDoorActionType {Automatic,Buttons };

	MeshInstance3D meshDoorRight1;
	MeshInstance3D meshDoorRight2;

    AnimationPlayer animPlayer;
    AudioStreamPlayer3D audioPlayer3D;

    bool open;
    bool last_needOpenState;
    bool animation_finish = true;

    [Export] bool _open { get { return open;} set {open = value; SetInstantOpen(open); } }
    [Export] float speed = 1.0f;

    [Export] EDoorActionType doorActionType = EDoorActionType.Automatic;

    [ExportGroupAttribute("For Automatic Settings")]
    [Export] bool a;

    [ExportGroupAttribute("For Buttons Settings")]
    [Export] public Array<NodePath> buttonsPaths;

    [ExportGroupAttribute("Sounds")]
    [Export] AudioStream openDoorAudioStream = null;
    [Export] AudioStream closeDoorAudioStream = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		meshDoorRight1 = GetNode<MeshInstance3D>("door");
        meshDoorRight2 = GetNode<MeshInstance3D>("door2");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        audioPlayer3D = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

        last_needOpenState = open;

        SetInstantOpen(open);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public void UpdateActualState()
    {
        if(last_needOpenState != open)
        {
            //update new state
            SetAnimOpenDoor(last_needOpenState);
            PlayAudioOpenDoor(last_needOpenState);
        }
    }

    public void SetInstantOpen(bool newOpen)
    {
        if (meshDoorRight1 == null || meshDoorRight2 == null) return;

        if (newOpen)
        {
            //open
            meshDoorRight1.Position = new Vector3(-0.8f, 0, 0);
            meshDoorRight2.Position = new Vector3(0.8f, 0, 0);
        }
        else
        {
            //close
            meshDoorRight1.Position = new Vector3(0, 0, 0);
            meshDoorRight2.Position = new Vector3(0, 0, 0);
        }

        open = newOpen;
    }

    public void SetAnimOpenDoor(bool newOpen)
    {
        if (meshDoorRight1 == null || meshDoorRight2 == null || animPlayer == null) return;

        if (newOpen)
        {
            //open
            animPlayer.Play("open_door",-1,speed);
            PlayAudioOpenDoor(true);
        }
        else
        {
            //close
            animPlayer.Play("close_door",-1,speed);
            PlayAudioOpenDoor(false);
        }

        animation_finish = false;
        open = newOpen;
    }

    public void PlayAudioOpenDoor(bool newOpen)
    {
        if (audioPlayer3D == null) return;

        if(newOpen)
        {
            audioPlayer3D.Stream = openDoorAudioStream;
            audioPlayer3D.Play();
        }
        else
        {
            audioPlayer3D.Stream = closeDoorAudioStream;
            audioPlayer3D.Play();
        }
    }

    public void _on_area_3d_body_entered(Node3D a)
    {
        if (a.IsClass("CharacterBody3D") && doorActionType == EDoorActionType.Automatic)
        {
            last_needOpenState = true;

            if(animation_finish)
            {
                GD.Print("door opened");
                SetAnimOpenDoor(true);
            }
        }
    }

    public void _on_area_3d_body_exited(Node3D a)
    {
        if (a.IsClass("CharacterBody3D") && doorActionType == EDoorActionType.Automatic)
        {
            last_needOpenState = false;

            if (animation_finish)
            {
                GD.Print("door closed");
                SetAnimOpenDoor(false);
            }
        }
    }

    public void _on_animation_player_animation_finished(string animName)
    {
        GD.Print(animName+ " animation finished");
        animation_finish = true;

        if(doorActionType == EDoorActionType.Automatic)
            UpdateActualState();
    }

    public void UseActionByButton()
    {
        if(doorActionType == EDoorActionType.Buttons)
        {
            if(animation_finish)
            {
                GD.Print("UseAction by button (open door)");
                SetAnimOpenDoor(!open);
                PlayAudioOpenDoor(!open);
            }
        }
    }
}
