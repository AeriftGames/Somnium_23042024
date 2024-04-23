using Godot;
using System;
using System.Transactions;

public partial class monitor : Node3D
{
	[Export] int NumberFrameToWaitToNextRender = 10;
    [Export] int NumberFrameToWaitToNextRenderNotActive = 30;

	[Export] SubViewport CamSubViewport;
	bool isInArea = false;
	bool isInScreen = false;
    private int ActualFrameToWait = 0;

	public override void _Ready()
	{
    }

	public override void _Process(double delta)
	{
		CheckCulling();
	}

	private void CheckCulling()
	{
		if (CamSubViewport == null) return;

		if(isInScreen && isInArea)
		{
			if (ActualFrameToWait >= NumberFrameToWaitToNextRender)
			{
                CamSubViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
				ActualFrameToWait = 0;
            }
			else 
			{ ActualFrameToWait++; }
		}
		else
		{
            if (ActualFrameToWait >= NumberFrameToWaitToNextRenderNotActive)
            {
                CamSubViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
                ActualFrameToWait = 0;
            }
            else
			{ ActualFrameToWait++; }
        }
	}

	public void _on_area_3d_body_entered(Node3D a){isInArea = true;}
	public void _on_area_3d_body_exited(Node3D a){isInArea = false;}
	public void _on_visible_on_screen_notifier_3d_screen_entered(){isInScreen = true;}
	public void _on_visible_on_screen_notifier_3d_screen_exited(){isInScreen = false;}
}
