using Godot;
using System;

public partial class DebugLabels : PanelContainer
{
	private VBoxContainer PropertyContainer;

	public override void _Ready()
	{
		PropertyContainer = GetNode<VBoxContainer>("MarginContainer/PropertyContainer");

		CGameMaster.GM.GetUniversal().SetDebugLabels(this);
	}

	public override void _Process(double delta)
	{
	}

	public void AddProperty(string newTitle, string newValue, int newOrder)
	{
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
