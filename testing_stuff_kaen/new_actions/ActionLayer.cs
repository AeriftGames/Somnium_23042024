using Godot;
using System;

public partial class ActionLayer : Control
{
	private FpsCharacterBase Character;
	private AnimationPlayer player;

	private bool isActivate = false;

	private ColorRect SelectRect;
	private ColorRect LeftUpRect;
	private ColorRect RightUpRect;
	private ColorRect LeftDownRect;
	private ColorRect RightDownRect;
	private ColorRect ActionRect;

	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer_ActionLayer");

		SelectRect = GetNode<ColorRect>("SelectRect");

		LeftUpRect = SelectRect.GetNode<ColorRect>("LeftUpRect");
        RightUpRect = SelectRect.GetNode<ColorRect>("RightUpRect");
        LeftDownRect = SelectRect.GetNode<ColorRect>("LeftDownRect");
        RightDownRect = SelectRect.GetNode<ColorRect>("RightDownRect");
		ActionRect = SelectRect.GetNode<ColorRect>("ActionRect");

        DeactiveObjectActionLayer();
    }
	
	public void ActivateObjectActionLayer(Vector3 LeftUp,Vector3 RightUp,Vector3 LeftDown,Vector3 RightDown)
	{
        Character = CGameMaster.GM.GetGame().GetFPSCharacterBase();

		// Select Rect
		LeftUpRect.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(LeftUp);
        RightUpRect.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(RightUp);
        LeftDownRect.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(LeftDown);
        RightDownRect.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(RightDown);


		// Action Rect
		Vector3 rightUpToRightDown = RightUp.DirectionTo(RightDown);
		float distance = RightUp.DistanceTo(RightDown);
		Vector3 newPos = RightUp + (rightUpToRightDown * (distance / 2));

		ActionRect.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(newPos);


        if (isActivate == false)
        {
            SelectRect.Visible = true;

            player.Play("StartShow");
            isActivate = true;
        }
    }
	
	public void DeactiveObjectActionLayer() 
	{
		player.Play("RESET");
		isActivate = false;

		SelectRect.Visible = false;
	}
}
