using Godot;
using System;

public partial class InventoryItemDataNode : Node3D
{
	[Export] public InventoryItemData Data;
	[Export] public float pickupSpeed = 0.2f;
	[Export] public float pickupHeight = 0.8f;
	[Export] public AudioStream sfx;

	private AudioStreamPlayer audioStreamPlayer = null;

	private bool isUsed = false;
	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioStreamPlayer.Stream = sfx;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Use()
	{
		if (isUsed) return;

        UseWantAddToInventory();
	}

	public void UseWantAddToInventory()
	{
		FPSCharacter_Inventory charInventory = CGameMaster.GM.GetGame().GetFPSCharacterOld() as FPSCharacter_Inventory;
		if (charInventory == null) return;

		// pokusime se pridat item do inventare hrace
		if(charInventory.GetInventoryComponent().AddItemToInventory(Data) == false)
		{
			// inventar je plny
			GD.Print("Inventar je plny");
			return;
		}

		// spustime tween animaci pickup
        StartTweenPickup();
		isUsed = true;
    }

	public void StartTweenPickup()
	{
        RigidBody3D b = GetParent() as RigidBody3D;
        if (b != null)
        {
            GD.Print("tento objekt je RigidBody3D");

            FPSCharacter_Inventory charInventory = CGameMaster.GM.GetGame().GetFPSCharacterOld() as FPSCharacter_Inventory;
            if (charInventory == null) return;

			Vector3 playerPos = charInventory.GlobalPosition;
            float playerHeight = playerPos.Y + pickupHeight;

			audioStreamPlayer.Play();

			b.SetPhysicsProcess(false);

			Tween tweenPos = CreateTween();
			tweenPos.TweenProperty(b, "global_position", 
				new Vector3(playerPos.X, playerHeight, playerPos.Z), pickupSpeed);

			// kdyz se dokonci tweeb - spusti se funkce
			tweenPos.Finished += TweenFinish;

            return;
        }

        Node3D a = GetParent() as Node3D;
		if(a != null)
		{
			GD.Print("tento objekt je Node3D");
			return;
		}
    }

	public void TweenFinish()
	{
		// skryjeme objekt
		Node3D parent = GetParent() as Node3D;
		parent.Hide();
    }

    public void _on_audio_stream_player_finished()
	{
		// znicime objekt
       GetParent().QueueFree();
	}
}
