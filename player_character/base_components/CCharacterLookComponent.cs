using Godot;

public partial class CCharacterLookComponent : Node
{
	[Export] public Camera3D Camera;
	[Export] public float MOUSE_SENSITIVITY = 0.3f;
	[Export] public float MOUSE_LERPSPEED = 15.0f;
	[Export] public float TILT_LOWER_LIMIT = Mathf.DegToRad(-90.0f);
    [Export] public float TILT_UPPER_LIMIT = Mathf.DegToRad(90.0f);

	private FpsCharacterBase ourCharacter = null;
    private bool isMouseInput = false;
    private float rotationInput;
    private float tiltInput;
	private Vector3 mouseRotation;

	private Vector3 playerRotation;
	private Vector3 cameraRotation;

	private float OnlyStartOffset = 0.0f;

	private Node3D LookingPoint = null;
	private Node3D RightHandPoint = null;

	public void PostInit(FpsCharacterBase newOurCharacter)
	{
		ourCharacter = newOurCharacter;

		LookingPoint = GetMainCamera().GetNode<Node3D>("LookPoint");
		RightHandPoint = GetMainCamera().GetNode<Node3D>("RightHandPoint");

        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

		isMouseInput = @event is InputEventMouseMotion eventMouseMotion_test && Input.MouseMode == Input.MouseModeEnum.Captured;

        if (isMouseInput)
		{
            InputEventMouseMotion eventMouseMotion = @event as InputEventMouseMotion;
            rotationInput = -eventMouseMotion.Relative.X * MOUSE_SENSITIVITY;
			tiltInput = -eventMouseMotion.Relative.Y * MOUSE_SENSITIVITY;
		}
	}

	public void UpdateLook(double delta)
	{
		mouseRotation.X += tiltInput * (float)delta;
		mouseRotation.X = Mathf.Clamp(mouseRotation.X,TILT_LOWER_LIMIT,TILT_UPPER_LIMIT);
		mouseRotation.Y += rotationInput * (float)delta;

		playerRotation = playerRotation.Lerp(new Vector3(0.0f, mouseRotation.Y, 0.0f),(float)delta*MOUSE_LERPSPEED);
		ourCharacter.Basis = Basis.FromEuler(playerRotation);

		cameraRotation = cameraRotation.Lerp(new Vector3(mouseRotation.X, 0.0f, 0.0f),(float)delta*MOUSE_LERPSPEED);
		Camera.Basis = Basis.FromEuler(cameraRotation);

		rotationInput = 0.0f;
		tiltInput = 0.0f;
	}

	public void RotateStart(Vector3 newStartRot)
	{
		mouseRotation.Y = newStartRot.Y;
    }

	public Camera3D GetMainCamera() { return Camera; }

	public Vector3 GetMainCameraLookingPoint() { return LookingPoint.GlobalPosition; }
    public Vector3 GetRightHandPoint() { return RightHandPoint.GlobalPosition; }
}
