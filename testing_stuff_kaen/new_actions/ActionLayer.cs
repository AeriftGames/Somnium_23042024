using Godot;
using System;

public partial class ActionLayer : Control
{
	private ColorRect BoxTest;
	private FpsCharacterBase Character;

	private AnimationPlayer player;
	private bool isActivate = false;

	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer_ActionLayer");

		BoxTest = GetNode<ColorRect>("BoxTest");
		DeactiveObjectActionLayer();

    }

	public override void _Process(double delta)
	{
	}

	public void ActiveObjectActionLayer(Vector3 min,Vector3 max,Vector3 pos)
	{
        Character = CGameMaster.GM.GetGame().GetFPSCharacterBase();
        BoxTest.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(min);
		BoxTest.Size = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(max) - BoxTest.Position;

		if (isActivate == false)
		{
            BoxTest.Visible = true;
            player.Play("StartShow");
            isActivate = true;
        }
    }

	public void DeactiveObjectActionLayer() 
	{
		BoxTest.Visible = false;
		player.Play("RESET");
		isActivate = false;
	}
}
