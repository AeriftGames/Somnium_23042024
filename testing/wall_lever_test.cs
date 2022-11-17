using Godot;
using System;

public partial class wall_lever_test : Node3D
{
	public interactive_object interactiveObject = null;
	public FPSCharacter_Interaction interactCharacter = null;

	RigidBody3D leverGrab = null;
	HingeJoint3D hingeJoint3D = null;

	bool isActionUpdate = false;

	public override void _Ready()
	{
		interactiveObject = GetNode<interactive_object>("LeverGrab/interactive_object");

		leverGrab = GetNode<RigidBody3D>("LeverGrab");
		hingeJoint3D = GetNode<HingeJoint3D>("HingeJoint3D");


	}
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if (@event is InputEventMouseMotion && isActionUpdate)
        {
            InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;
            var a = mouseEventMotion.Relative;
            
        }
    }

    public override void _Process(double delta)
	{
        GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "rotaceX= " + Mathf.RadToDeg(leverGrab.Rotation.x));

        // odsud dolu je funkce pri grab action
        if (!isActionUpdate) return;

		if(Input.IsActionJustPressed("testTop"))
		{
			SetActualPositon(true);
		}

        if (Input.IsActionJustPressed("testBottom"))
        {
			SetActualPositon(false);
        }

        // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
        float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);

        if (actual_rot >= 44.0f)
            DetectReachEndPoint(false);
        else if (actual_rot <= -44.0f)
            DetectReachEndPoint(true);
    }

	public void DetectReachEndPoint(bool newTop)
	{
        SetReachPositionFreeze(newTop);

        if (newTop)
		{
			GD.Print("Lever is in Top End positon !");
		}
		else
		{
            GD.Print("Lever is in Bottom End positon !");
        }
	}

	public void SetActualPositon(bool newTop)
	{
		if(newTop)
		{
            Vector3 oldRot = leverGrab.Rotation;
			oldRot.x = Mathf.DegToRad(-45.0f);
			leverGrab.Rotation = oldRot;
        }
		else
		{
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(45.0f);
            leverGrab.Rotation = oldRot;
        }
	}

	public void SetReachPositionFreeze(bool newTop)
	{
		if(newTop)
		{
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(-45.0f);
            leverGrab.Rotation = oldRot;
        }
		else
		{
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(45.0f);
            leverGrab.Rotation = oldRot;
        }

		leverGrab.CanSleep = true;
		leverGrab.Sleeping = true;
	}

    public virtual void message_update()
    {
        string msg = interactiveObject.msgObject.GetMessage();
        switch (msg)
        {
			case "msg_grab_action_start": 
				{
                    interactCharacter = (FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData();
                    isActionUpdate = true;
                    break;
				}
			case "msg_grab_action_end":
				{
                    interactCharacter = (FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData();
					isActionUpdate = false;
                    break;
				}
            default: break;
        }
    }
}
