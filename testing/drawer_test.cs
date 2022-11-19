using Godot;
using System;

public partial class drawer_test : Node3D
{
	[Export] public float linearLimitZ = -0.5f;
	[Export] public float mouseMotionSpeed = 0.2f;
    [Export] public float linearVelocityLimit = 1.6f;

	private interactive_object interactiveObject = null;
	private FPSCharacter_Interaction interactCharacter = null;
    private bool isActionUpdate = false;
	private Vector2 motionMouse = Vector2.Zero;

    private RigidBody3D drawerGrab = null;
	private Generic6DOFJoint3D genericJoint = null;

	public override void _Ready()
	{
		interactiveObject = GetNode<interactive_object>("DrawerGrab/interactive_object");

		drawerGrab = GetNode<RigidBody3D>("DrawerGrab");
		genericJoint = GetNode<Generic6DOFJoint3D>("GenericJoint");

		genericJoint.SetParamZ(Generic6DOFJoint3D.Param.LinearLowerLimit,linearLimitZ);

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
        // pokud jsme v GrabAction pustime update
        if (isActionUpdate)
        {
            UpdateDrawer(delta);
        }

        // Reset for zero
        motionMouse = Vector2.Zero;
    }

    public void UpdateDrawer(double delta)
    {
        // nastavime velocity podle motion mouse
        var newVel = drawerGrab.GlobalTransform.basis.z.Normalized() * motionMouse.y * mouseMotionSpeed;

        // nastavime maximalni limit rychlosti - rigidbody linear velocity
        if (Mathf.Abs(newVel.Length()) > linearVelocityLimit)
            newVel = newVel.LimitLength(linearVelocityLimit);

        drawerGrab.LinearVelocity = newVel;
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
