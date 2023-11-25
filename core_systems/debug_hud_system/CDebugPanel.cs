using Godot;
using System;

public partial class CDebugPanel : Control
{
	private bool isOpen = false;

	public void PostInit()
	{

	}

	public void ToggleOpen(){ OpenDebugPanel(!isOpen); }

	public void OpenDebugPanel(bool newOpen)
	{
		isOpen = newOpen;

		if (isOpen)
		{
			Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Visible = false;
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
	}
}
