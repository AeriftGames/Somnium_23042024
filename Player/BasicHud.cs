using Godot;
using System;

public partial class BasicHud : Control
{
	Label UseLabel = null;

	public override void _Ready()
	{
		UseLabel = GetNode<Label>("Crosshair/UseLabel");
	}

	public void SetUseVisible(bool newVisible)
	{
		UseLabel.Visible = newVisible;
	}
}
