using Godot;
using System;

public partial class BasicHud : Control
{
	Label UseLabel = null;
	TextureRect handGrabTexture = null;

	public override void _Ready()
	{
		UseLabel = GetNode<Label>("Crosshair/UseLabel");
		handGrabTexture = GetNode<TextureRect>("HandGrabTexture");

		SetHandGrabVisible(false);
	}

	public void SetUseVisible(bool newVisible)
	{
		UseLabel.Visible = newVisible;
	}

	public void SetUseLabelText(string newText)
	{
		UseLabel.Text = newText;
	}

	public void SetHandGrabVisible(bool newVisible)
	{
		handGrabTexture.Visible = newVisible;
	}

	public bool GetHandGrabVisible()
	{
		return handGrabTexture.Visible;
	}
}
