using Godot;
using System;

public partial class wardrobe_small : Node3D
{
    public enum EGrabMoveType { Set, Add }

    [Export] public float mouseMotionSpeed = 0.003f;
    [Export] public float linearVelocityLimit = 2.0f;
    [Export] public EGrabMoveType grabMoveType = EGrabMoveType.Add;

    private interactive_object interactiveObject = null;
    private FPSCharacter_Interaction interactCharacter = null;
    private bool isActionUpdate = false;
    private Vector2 motionMouse = Vector2.Zero;

    private RigidBody3D wardrobeDoorGrab = null;
    private HingeJoint3D hingeJoint = null;

    private Node3D targetMove = null; 

    bool mouseUpdated = false;

    public override void _Ready()
	{
        interactiveObject = GetNode<interactive_object>("WardrobeDoorGrab/interactive_object");

        wardrobeDoorGrab = GetNode<RigidBody3D>("WardrobeDoorGrab");
        hingeJoint = GetNode<HingeJoint3D>("HingeJoint3D");

        targetMove = GetNode<Node3D>("TargetMove");
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
            UpdateDoor(delta);
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
        mouseUpdated = false;
    }

    public void UpdateDoor(double delta)
    {
        var newVel = wardrobeDoorGrab.GlobalPosition.DirectionTo(
            targetMove.GlobalPosition).Normalized() * motionMouse.y * mouseMotionSpeed;

        switch (grabMoveType)
        {
            case EGrabMoveType.Set:
                {
                    wardrobeDoorGrab.LinearVelocity = newVel;
                    break;
                }
            case EGrabMoveType.Add:
                {
                    if (mouseUpdated)
                        wardrobeDoorGrab.LinearVelocity += newVel;
                    break;
                }
        }

        // pokud je linear velocity vyysi nez pozadovany limit, nastavime hodnotu z limitu
        if (Mathf.Abs(wardrobeDoorGrab.LinearVelocity.Length()) > linearVelocityLimit)
            wardrobeDoorGrab.LinearVelocity = wardrobeDoorGrab.LinearVelocity.LimitLength(linearVelocityLimit);
    }

    public void ActionStart()
    {

    }

    public void ActionEnd()
    {

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
