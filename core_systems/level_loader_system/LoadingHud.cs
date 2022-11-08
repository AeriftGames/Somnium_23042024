using Godot;
using System;

public partial class LoadingHud : Control
{
	Label nameOfLevelLabel = null;

	public override void _Ready()
	{
		nameOfLevelLabel = GetNode<Label>("NameOfLevelLabel");
	}

	public void SetInitializeAndVisibleNow(string newNameOfLevel, bool newShowShaderProcess)
	{
		nameOfLevelLabel.Text = newNameOfLevel;
	}

	public void LoadingIsComplete(bool newShowPressAnyKeyToStart)
	{
		QueueFree();
	}
}
