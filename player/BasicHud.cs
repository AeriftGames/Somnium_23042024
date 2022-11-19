using Godot;
using System;

public partial class BasicHud : Control
{
	ColorRect Crosshair = null;
	Label UseLabel = null;
	TextureRect handGrabbedTexture = null;
    TextureRect handCanGrabTexture = null;

    public override void _Ready()
	{
		Crosshair = GetNode<ColorRect>("Crosshair");
		UseLabel = GetNode<Label>("UseLabel");
		handGrabbedTexture = GetNode<TextureRect>("HandGrabbedTexture");
        handCanGrabTexture = GetNode<TextureRect>("HandCanGrabTexture");

		// visible = false , grabbed = false
		SetHandGrabState(false,false);
	}

	public void SetCrosshairVisible(bool newVisible)
	{
		Crosshair.Visible = newVisible;
	}

	public bool GetCrosshairVisible()
	{
		return Crosshair.Visible;
	}

	public void SetUseVisible(bool newVisible)
	{
		UseLabel.Visible = newVisible;
	}

	public void SetUseLabelText(string newText)
	{
		UseLabel.Text = newText;
	}

	private void SetHandGrabbedVisible(bool newVisible)
	{
        handGrabbedTexture.Visible = newVisible;
	}

    private void SetHandCanGrabVisible(bool newVisible)
    {
        handCanGrabTexture.Visible = newVisible;
    }

    public bool GetHandGrabbedVisible()
	{
		return handGrabbedTexture.Visible;
	}

    public bool GetHandCanGrabVisible()
    {
        return handCanGrabTexture.Visible;
    }

    // grabbed true = texture grabbed and false = texture normal
    public void SetHandGrabState(bool newVisible,bool newGrabbed)
	{
		if(newVisible)
		{
			if(newGrabbed)
			{
                handGrabbedTexture.Visible = true;
                handCanGrabTexture.Visible = false;
            }
			else
			{
                handGrabbedTexture.Visible = false;
                handCanGrabTexture.Visible = true;
            }
		}
		else
		{
			handGrabbedTexture.Visible = false;
            handCanGrabTexture.Visible = false;
        }
	}

	public TextureRect GetHandGrabbedTextureRect()
	{
		return handGrabbedTexture;
	}
}
