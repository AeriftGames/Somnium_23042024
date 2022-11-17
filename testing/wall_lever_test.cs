using Godot;
using System;

public partial class wall_lever_test : Node3D
{
    public interactive_object interactiveObject = null;
    public FPSCharacter_Interaction interactCharacter = null;

    RigidBody3D leverGrab = null;
    HingeJoint3D hingeJoint3D = null;

    bool isActionUpdate = false;

    //
    Vector2 motionMouse = Vector2.Zero;
    [Export] public float MotorPower = 3.0f;
    float max = 60.0f;
    float min = -60.0f;
    bool isReach = false;

    public override void _Ready()
    {
        interactiveObject = GetNode<interactive_object>("LeverGrab/interactive_object");

        leverGrab = GetNode<RigidBody3D>("LeverGrab");
        hingeJoint3D = GetNode<HingeJoint3D>("HingeJoint3D");

        SetReachPositionFreeze(true);
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion && isActionUpdate)
        {
            InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;
            motionMouse = mouseEventMotion.Relative;
        }
    }

    public override void _Process(double delta)
    {
        // pokud jsme v GrabAction
        if (isActionUpdate)
        {
            if (Input.IsActionJustPressed("testTop"))
            {
                SetActualPositon(true);
            }

            if (Input.IsActionJustPressed("testBottom"))
            {
                SetActualPositon(false);
            }

            // Pohyb mysi
            leverGrab.RotateX(Mathf.DegToRad(motionMouse.y * 0.2f));

            // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
            float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);

            if (actual_rot >= max-1.0f)
                DetectReachEndPoint(false);
            else if (actual_rot <= min+1.0f)
                DetectReachEndPoint(true);
            else
            {
                // normalni chod
                leverGrab.Freeze = false;
                leverGrab.Sleeping = false;
                isReach = false;
            }
        }
        else
        {
            if(!isReach)
            {
                // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
                float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);

                if (actual_rot >= max - 1.0f)
                    DetectReachEndPoint(false);
                else if (actual_rot <= min + 1.0f)
                    DetectReachEndPoint(true);
            }
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
    }

    public void DetectReachEndPoint(bool newTop)
    {
        SetReachPositionFreeze(newTop);
        isReach = true;
    }

    public void SetActualPositon(bool newTop)
    {
        if (newTop)
        {
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(min);
            leverGrab.Rotation = oldRot;
        }
        else
        {
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(max);
            leverGrab.Rotation = oldRot;
        }
    }

    public void SetReachPositionFreeze(bool newTop)
    {
        if (newTop)
        {
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(min);
            leverGrab.Rotation = oldRot;
        }
        else
        {
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(max);
            leverGrab.Rotation = oldRot;
        }

        leverGrab.CanSleep = true;
        leverGrab.Sleeping = true;
        leverGrab.Freeze = true;
    }

    public void SetMotorToPosition(bool newTop)
    {
        if (newTop)
        {
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorTargetVelocity, 1.0f * MotorPower);
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorMaxImpulse, 1.0f);
            hingeJoint3D.SetFlag(HingeJoint3D.Flag.EnableMotor, true);
        }
        else
        {
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorTargetVelocity, -1.0f * MotorPower);
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorMaxImpulse, 1.0f);
            hingeJoint3D.SetFlag(HingeJoint3D.Flag.EnableMotor, true);
        }
    }

    public void SetMotorDisable()
    {
        hingeJoint3D.SetFlag(HingeJoint3D.Flag.EnableMotor, false);
    }

    public void ActionStart()
    {

    }

    public void ActionEnd()
    {
        // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
        float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);

        //Spustime motor tim smerem podle toho kde se aktualne nachazi paka

        if (actual_rot >= 0.0f)
            SetMotorToPosition(true);
        else if (actual_rot <= -0.0001f)
            SetMotorToPosition(false);
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
                    ActionStart();
                    break;
				}
			case "msg_grab_action_end":
				{
                    interactCharacter = (FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData();
					isActionUpdate = false;
                    ActionEnd();
                    break;
				}
            default: break;
        }
    }
}
