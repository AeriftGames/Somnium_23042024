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

	private Control ActionObjectControl;

	private Label ActionObjectNameLabel;
	private VBoxContainer ActionElementsVbox;
	/*
	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer_ActionLayer");

		SelectRect = GetNode<ColorRect>("SelectRect");

		LeftUpRect = SelectRect.GetNode<ColorRect>("LeftUpRect");
        RightUpRect = SelectRect.GetNode<ColorRect>("RightUpRect");
        LeftDownRect = SelectRect.GetNode<ColorRect>("LeftDownRect");
        RightDownRect = SelectRect.GetNode<ColorRect>("RightDownRect");
		ActionRect = SelectRect.GetNode<ColorRect>("ActionRect");

		ActionObjectControl = SelectRect.GetNode<Control>("ActionObjectControl");

		ActionObjectNameLabel = SelectRect.GetNode<Label>("%ActionObjectNameLabel");
		ActionElementsVbox = SelectRect.GetNode<VBoxContainer>("%ActionElementsVBox");

        DeactiveObjectActionLayer();
    }
    //Vector3 LeftUp, Vector3 RightUp,Vector3 LeftDown, Vector3 RightDown
    public void ActivateObjectActionLayer(CInteractiveObject newInteractiveObject)
	{
		if(newInteractiveObject == null) return;
		if (newInteractiveObject.GetBilboardObject() == null) return;

		Vector3 LeftUp = newInteractiveObject.GetBilboardObject().GetLeftUpPosition();
        Vector3 RightUp = newInteractiveObject.GetBilboardObject().GetRightUpPosition();
        Vector3 LeftDown = newInteractiveObject.GetBilboardObject().GetLeftDownPosition();
        Vector3 RightDown = newInteractiveObject.GetBilboardObject().GetRightDownPosition();

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

        // ActionObjectContainer
        Vector3 leftUpToRightUp = LeftUp.DirectionTo(RightUp);
        float distance_leftToRight = LeftUp.DistanceTo(RightUp);
        Vector3 newPos_centerUp = LeftUp + (leftUpToRightUp * (distance_leftToRight / 2));

        ActionObjectControl.Position = Character.GetCharacterLookComponent().GetMainCamera().UnprojectPosition(newPos_centerUp);

        // ONCE Activate
        if (isActivate == false)
        {
			SetDataFromInteractiveObject(newInteractiveObject);

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

		DeleteAllActionElements();
    }

	public void SetDataFromInteractiveObject(CInteractiveObject newInteractiveObject)
	{
		if(newInteractiveObject == null) return;

		// SET OBJECT NAME
		ActionObjectNameLabel.Text = newInteractiveObject.GetObjectName();

		// SET ACTIONS
		PackedScene prefabActionElement = 
			GD.Load<PackedScene>("res://testing_stuff_kaen/new_actions/action_ui_element.tscn");

		if (newInteractiveObject.GetAllActionResources() != null)
		{
            ActionRect.Visible = true;

            foreach (var action_resource in newInteractiveObject.GetAllActionResources())
            {
                // spawn element
                CActionUIElement actionElement = prefabActionElement.Instantiate<CActionUIElement>();
                ActionElementsVbox.AddChild(actionElement);

                // set data
                actionElement.StartInit();
                actionElement.SetData(action_resource.ActionName, action_resource.ActionInputActivate);
            }
		}
		else
		{
            // POKUD NEEXISTUJI ZADNE AKCE
            ActionRect.Visible = false;
        }
	}

	public void DeleteAllActionElements()
	{
		foreach (Control action_element in ActionElementsVbox.GetChildren())
        {
			action_element.Visible = false;
			action_element.QueueFree();
		}
	}*/
}
