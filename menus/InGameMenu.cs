using Godot;
using System;

public partial class InGameMenu : Control
{
	private bool active = false;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void SetActive(bool newActive)
	{
		active = newActive;
		Visible = active;
	}

	public bool GetActive() { return active; }
}
