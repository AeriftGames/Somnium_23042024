using Godot;
using System;

public partial class CDebugLabels : PanelContainer
{
	private VBoxContainer PropertyContainer;
	private bool isActive = false;

	public override void _Ready()
	{
		PropertyContainer = GetNode<VBoxContainer>("MarginContainer/PropertyContainer");
	}
	public void SetActive(bool newActive)
	{
		isActive = newActive;

		if ( isActive ){ Visible = true; }
		else { Visible = false; }
	}
	public bool GetIsActive() { return isActive; }
	public void AddProperty(string newTitle, string newValue, int newOrder)
	{
		if (isActive == false) return;

		Node target = PropertyContainer.FindChild(newTitle, true, false);
		if(target == null)
		{
			target = new Label();
			PropertyContainer.AddChild(target);
			target.Name = newTitle;
			target.Set("text", target.Name + ": "+newValue);
		}
		else if(Visible)
		{
			target.Set("text", newTitle + ": " + newValue);
			PropertyContainer.MoveChild(target, newOrder);
		}
	}
}
