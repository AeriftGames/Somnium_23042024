using Godot;
using System;

public partial class drawer_test : Node3D
{
    public enum EGrabMoveType { Set, Add }

    [ExportGroupAttribute("Drawer: only onready set param")]
    [Export] public Vector3 drawerInertia = new Vector3(0.01f, 0.01f, 0.01f);
    [Export] public float drawerLinearDamp = 4.0f;
    [Export] public float drawerAngularDamp = 4.0f;
	[Export] public float linearLimitZ = -0.5f;
    [Export] public float drawerMass = 20.0f;

    [ExportGroupAttribute("Drawer: set param")]
    [Export] public float mouseMotionSpeed = 0.003f;
    [Export] public float linearVelocityLimit = 2.0f;
    [Export] public EGrabMoveType grabMoveType = EGrabMoveType.Add;

	private interactive_object interactiveObject = null;
	private FPSCharacter_Interaction interactCharacter = null;
    private bool isActionUpdate = false;
	private Vector2 motionMouse = Vector2.Zero;

    private RigidBody3D drawerGrab = null;
	private Generic6DofJoint3D genericJoint = null;

    bool mouseUpdated = false;

	public override void _Ready()
	{
		interactiveObject = GetNode<interactive_object>("DrawerGrab/interactive_object");

		drawerGrab = GetNode<RigidBody3D>("DrawerGrab");
		genericJoint = GetNode<Generic6DofJoint3D>("GenericJoint");

        drawerGrab.Inertia = drawerInertia;
        drawerGrab.LinearDamp = drawerLinearDamp;
        drawerGrab.AngularDamp = drawerAngularDamp;
		genericJoint.SetParamZ(Generic6DofJoint3D.Param.LinearLowerLimit,linearLimitZ);
        drawerGrab.Mass = drawerMass;
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
    /*
    public override void _Process(double delta)
	{
        // pokud jsme v GrabAction pustime update
        if (isActionUpdate)
        {
            UpdateDrawer(delta);
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
        mouseUpdated = false;
    }*/

    public override void _PhysicsProcess(double delta)
    {
        // pokud jsme v GrabAction pustime update
        if (isActionUpdate)
        {
            UpdateDrawer(delta);
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
        mouseUpdated = false;
    }

    public void UpdateDrawer(double delta)
    {
        // nastavime velocity podle motion mouse
        var newVel = drawerGrab.GlobalTransform.Basis.Z.Normalized() * motionMouse.Y * mouseMotionSpeed;

        switch (grabMoveType) 
        {
            case EGrabMoveType.Set: 
                {
                    drawerGrab.LinearVelocity = newVel;
                    break;
                }
            case EGrabMoveType.Add:
                {
                    if (mouseUpdated)
                        drawerGrab.LinearVelocity += newVel;
                    break;
                }
        }

        // pokud je linear velocity vyysi nez pozadovany limit, nastavime hodnotu z limitu
        if (Mathf.Abs(drawerGrab.LinearVelocity.Length()) > linearVelocityLimit)
            drawerGrab.LinearVelocity = drawerGrab.LinearVelocity.LimitLength(linearVelocityLimit);
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
