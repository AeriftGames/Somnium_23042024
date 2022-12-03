using Godot;
using System;

public partial class wall_lever_test : Node3D
{
    public enum EGrabMoveType { Set, Add }

    [Export] public float upperReachEnd = 60.0f;
    [Export] public float centerReachNeutral = 0.0f;
    [Export] public float lowerReachEnd = -60.0f;
    [Export] public float toleranceDetectReach = 2.0f;
    [Export] public float mouseMotionSpeed = 0.01f;
    [Export] public float motorPower = 3.0f;
    [Export] public float motorMaxImpulse = 1.0f;
    [Export] public float linearVelocityLimit = 15.0f;
    [Export] public EGrabMoveType grabMoveType = EGrabMoveType.Add;

    [Signal]
    public delegate void LeverReachEndEventHandler(bool newTop);

    protected interactive_object interactiveObject = null;
    protected FPSCharacter_Interaction interactCharacter = null;

    protected RigidBody3D leverGrab = null;
    private HingeJoint3D hingeJoint3D = null;

    protected bool isActionUpdate = false;
    protected Vector2 motionMouse = Vector2.Zero;
    protected bool mouseUpdated = false;

    public enum EReachPointEnd{Work,Bottom,Top}
    protected bool onceIsReachPoint = false;

    //LIGHTS TEST
    MeshInstance3D GreenLight = null;
    MeshInstance3D RedLight = null;

    //SOUND TEST
    AudioStreamPlayer3D audioStreamPlayer = null;

    public override void _Ready()
    {
        interactiveObject = GetNode<interactive_object>("LeverGrab/interactive_object");

        leverGrab = GetNode<RigidBody3D>("LeverGrab");
        hingeJoint3D = GetNode<HingeJoint3D>("HingeJoint3D");

        //LIGHTS
        GreenLight = GetNode<MeshInstance3D>("LeverStaticBody/MeshInstance3D/MeshInstanceGreen");
        RedLight = GetNode<MeshInstance3D>("LeverStaticBody/MeshInstance3D/MeshInstanceRed");

        //SOUND
        audioStreamPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

        //Initial Settings
        hingeJoint3D.SetParam(HingeJoint3D.Param.LimitUpper, Mathf.DegToRad(upperReachEnd));
        hingeJoint3D.SetParam(HingeJoint3D.Param.LimitLower, Mathf.DegToRad(lowerReachEnd));

        SetReachNow(EReachPointEnd.Bottom);
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMouseMotion && isActionUpdate)
        {
            InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;
            motionMouse = mouseEventMotion.Relative;
            mouseUpdated = true;
        }
    }

    public override void _Process(double delta)
    {
        // pokud jsme v GrabAction pustime update
        if (isActionUpdate)
        {
            UpdateLever(delta);
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
        mouseUpdated = false;
    }

    public virtual EReachPointEnd CalculateReaches()
    {
        // Vypocet pro detekci horniho konecneho bodu a spodniho konecneho bodu
        float actual_rot = Mathf.RadToDeg(leverGrab.Rotation.x);

        if(actual_rot >= upperReachEnd - toleranceDetectReach)
        {
            return EReachPointEnd.Bottom;
        }
        else if(actual_rot <= lowerReachEnd + toleranceDetectReach)
        {
            return EReachPointEnd.Top;
        }
        else
        {
            return EReachPointEnd.Work;
        }
    }

    public virtual void SetReachNow(EReachPointEnd newReachPoint)
    {
        switch(newReachPoint)
        {
            case EReachPointEnd.Top:
                {
                    var newRot = leverGrab.Rotation;
                    newRot.x = Mathf.DegToRad(lowerReachEnd - 3);
                    leverGrab.Rotation = newRot;
                    UpdateLever(0);
                    break; 
                }
            case EReachPointEnd.Bottom:
                {
                    var newRot = leverGrab.Rotation;
                    newRot.x = Mathf.DegToRad(upperReachEnd + 3);
                    leverGrab.Rotation = newRot;
                    UpdateLever(0);
                    break;
                }
            case EReachPointEnd.Work: 
                {
                    var newRot = leverGrab.Rotation;
                    newRot.x = Mathf.DegToRad(centerReachNeutral);
                    leverGrab.Rotation = newRot;
                    UpdateLever(0);
                    break;
                }
        }
    }

    public virtual void UpdateLever(double delta)
    {
        var newVel = -GlobalTransform.basis.y.Normalized() * motionMouse.y * mouseMotionSpeed;

        switch (grabMoveType)
        {
            case EGrabMoveType.Set:
                {
                    leverGrab.LinearVelocity = newVel;
                    break;
                }
            case EGrabMoveType.Add:
                {
                    if (mouseUpdated)
                        leverGrab.LinearVelocity += newVel;
                    break;
                }
        }

        // pokud je linear velocity vyysi nez pozadovany limit, nastavime hodnotu z limitu
        if (Mathf.Abs(leverGrab.LinearVelocity.Length()) > linearVelocityLimit)
            leverGrab.LinearVelocity = leverGrab.LinearVelocity.LimitLength(linearVelocityLimit);

        // vypocteme aktualni uhel a z nej vratime enum ktery rozhodne co se stane

        switch (CalculateReaches())
        {
            case EReachPointEnd.Top:
                {
                    if (onceIsReachPoint) return;

                    TestLight(true);
                    PlaySound(true);
                    EmitSignal(SignalName.LeverReachEnd, true);
                    onceIsReachPoint = true;
                    break;
                }
            case EReachPointEnd.Bottom:
                {
                    if (onceIsReachPoint) return;

                    TestLight(false);
                    PlaySound(true);
                    EmitSignal(SignalName.LeverReachEnd, false);
                    onceIsReachPoint = true;
                    break;
                }
            case EReachPointEnd.Work:
                {
                    onceIsReachPoint = false;
                    break;
                }
        }
    }

    public void SetMotorToPosition(bool newTop)
    {
        if (newTop)
        {
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorTargetVelocity, 1.0f * motorPower);
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorMaxImpulse, motorMaxImpulse);
            hingeJoint3D.SetFlag(HingeJoint3D.Flag.EnableMotor, true);
        }
        else
        {
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorTargetVelocity, -1.0f * motorPower);
            hingeJoint3D.SetParam(HingeJoint3D.Param.MotorMaxImpulse, motorMaxImpulse);
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

    public void PlaySound(bool newTop)
    {
        audioStreamPlayer.Play();
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
