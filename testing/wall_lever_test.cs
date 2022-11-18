using Godot;
using System;

public partial class wall_lever_test : Node3D
{
    [Export] public float MotorPower = 3.0f;
    [Export] public float max = 60.0f;
    [Export] public float center_value = 0.0f;
    [Export] public float min = -60.0f;
    [Export] public float motor_max_impulse = 1.0f;
    [Export] public float mouse_motion_speed = 0.2f;

    [Signal]
    public delegate void LeverReachEndEventHandler(bool newTop);

    private interactive_object interactiveObject = null;
    private FPSCharacter_Interaction interactCharacter = null;

    private RigidBody3D leverGrab = null;
    private HingeJoint3D hingeJoint3D = null;

    private bool isActionUpdate = false;
    private bool isAlreadyReached = false;
    private bool isInReach = false;
    private bool isReachTop = false;
    private Vector2 motionMouse = Vector2.Zero;

    public enum EReachPointEnd{Work,Bottom,Top}
    public EReachPointEnd EActualPoint;


    //LIGHTS
    MeshInstance3D GreenLight = null;
    MeshInstance3D RedLight = null;

    public override void _Ready()
    {
        interactiveObject = GetNode<interactive_object>("LeverGrab/interactive_object");

        leverGrab = GetNode<RigidBody3D>("LeverGrab");
        hingeJoint3D = GetNode<HingeJoint3D>("HingeJoint3D");

        //LIGHTS
        GreenLight = GetNode<MeshInstance3D>("LeverStaticBody/MeshInstance3D/MeshInstanceGreen");
        RedLight = GetNode<MeshInstance3D>("LeverStaticBody/MeshInstance3D/MeshInstanceRed");

        TestLight(true);
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
            var newVel = new Vector3(0, motionMouse.y * mouse_motion_speed, 0);
            leverGrab.LinearVelocity = -newVel;

            switch(CalculateReaches())
            {
                case EReachPointEnd.Top: 
                    {
                        TestLight(true);
                        break; 
                    }
                case EReachPointEnd.Bottom:
                    {
                        TestLight(false);
                        break;
                    }
                case EReachPointEnd.Work:
                    {

                        break;
                    }
            }


        }
        else
        {
            if(!isInReach)
            {
                // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
                float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);
            }
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
    }

    public EReachPointEnd CalculateReaches()
    {
        // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
        float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);

        if(actual_rot >= max-1)
        {
            return EReachPointEnd.Bottom;
        }
        else if(actual_rot <= min+1)
        {
            return EReachPointEnd.Top;
        }
        else
        {
            return EReachPointEnd.Work;
        }
    }

    public void DetectReachEndPoint(bool newTop)
    {
        SetReachPositionFreeze(newTop);
        isInReach = true;
    }

    public void SetReachPositionFreeze(bool newTop)
    {
        // FOR DO ONCE

        if (newTop)
        {
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(min);
            leverGrab.Rotation = oldRot;

            isReachTop = true;
        }
        else
        {
            Vector3 oldRot = leverGrab.Rotation;
            oldRot.x = Mathf.DegToRad(max);
            leverGrab.Rotation = oldRot;

            isReachTop = false;
        }

        leverGrab.CanSleep = true;
        leverGrab.Sleeping = true;
        leverGrab.Freeze = true;

        if (isAlreadyReached) return;

        // EmitSignal
        EmitSignal(SignalName.LeverReachEnd, newTop);

        // FOR DO ONCE
        isAlreadyReached = true;
    }

    public void SetMotorToPosition(bool newTop)
    {
        if (newTop)
        {
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorTargetVelocity, 1.0f * MotorPower);
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorMaxImpulse, motor_max_impulse);
            hingeJoint3D.SetFlag(HingeJoint3D.Flag.EnableMotor, true);
        }
        else
        {
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorTargetVelocity, -1.0f * MotorPower);
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorMaxImpulse, motor_max_impulse);
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

    }

    public void TestLight(bool newEnable)
    {
        if(newEnable)
        {
            //GREEN
            if (GreenLight == null || RedLight == null) return;
            GreenLight.Visible = true;
            RedLight.Visible = false;

        }
        else
        {
            //RED
            if (GreenLight == null || RedLight == null) return;
            GreenLight.Visible = false;
            RedLight.Visible = true;
        }
    }

    public void UpdateHud(float delta)
    {
       //interactCharacter.GetBasicHud()
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
