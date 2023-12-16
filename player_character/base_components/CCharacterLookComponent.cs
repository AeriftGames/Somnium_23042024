using Godot;

public partial class CCharacterLookComponent : CBaseComponent
{
	[Export] public Camera3D Camera;
	[Export] public float MOUSE_SENSITIVITY = 0.3f;
	[Export] public float MOUSE_LERPSPEED = 15.0f;
	[Export] public float TILT_LOWER_LIMIT = Mathf.DegToRad(-90.0f);
	[Export] public float TILT_UPPER_LIMIT = Mathf.DegToRad(90.0f);

	private bool isMouseInput = false;
	private float rotationInput;
	private float tiltInput;
	private Vector3 mouseRotation;

	private Vector3 playerRotation;
	private Vector3 cameraRotation;

	private float OnlyStartOffset = 0.0f;

	private Node3D CameraCrouch = null;
	private Node3D CameraLand = null;
	private Node3D CameraController = null;
	private Node3D CameraHead = null;
	private Node3D CameraSway = null;
	private Node3D CameraJump = null;
	private Node3D CameraShakeRot = null;
	private Node3D HeadBob = null;
	private Node3D HeadBreathing = null;
	private Node3D LookingPoint = null;
	private Node3D RightHandPoint = null;

	public override void PostInit(FpsCharacterBase newOurCharacter)
	{
		base.PostInit(newOurCharacter);

		CameraCrouch = ourCharacterBase.GetNode<Node3D>("%CameraCrouch");
		CameraLand = ourCharacterBase.GetNode<Node3D>("%CameraLand");
		CameraController = ourCharacterBase.GetNode<Node3D>("%CameraController");
		CameraHead = ourCharacterBase.GetNode<Node3D>("%CameraHead");
		CameraSway = ourCharacterBase.GetNode<Node3D>("%CameraSway");
		CameraJump = ourCharacterBase.GetNode<Node3D>("%CameraJump");
		CameraShakeRot = ourCharacterBase.GetNode<Node3D>("%CameraShakeRot");
		HeadBob = ourCharacterBase.GetNode<Node3D>("%HeadBob");
		HeadBreathing = ourCharacterBase.GetNode<Node3D>("%HeadBreathing");
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
		mouseRotation.X = Mathf.Clamp(mouseRotation.X, TILT_LOWER_LIMIT, TILT_UPPER_LIMIT);
		mouseRotation.Y += rotationInput * (float)delta;

		playerRotation = playerRotation.Lerp(new Vector3(0.0f, mouseRotation.Y, 0.0f), (float)delta * MOUSE_LERPSPEED);
		ourCharacterBase.Basis = Basis.FromEuler(playerRotation);

		cameraRotation = cameraRotation.Lerp(new Vector3(mouseRotation.X, playerRotation.Y, 0.0f), (float)delta * MOUSE_LERPSPEED);
		CameraHead.Basis = Basis.FromEuler(cameraRotation);

		rotationInput = 0.0f;
		tiltInput = 0.0f;

		UpdateCameraPosition(delta);
	}

	public void UpdateCameraPosition(double delta)
	{
		CameraHead.GlobalPosition =
			CameraHead.GlobalPosition.Lerp(CameraController.GlobalPosition, (float)delta * 50.0f); ;
	}

	public void RotateStart(Vector3 newStartRot) { mouseRotation.Y = newStartRot.Y; }

	// GETTIGNS
	public Camera3D GetMainCamera() { return Camera; }
	public Vector3 GetMainCameraLookingPointPos() { return LookingPoint.GlobalPosition; }
	public Vector3 GetRightHandPointPos() { return RightHandPoint.GlobalPosition; }
	public Node3D GetCameraHead() { return CameraHead; }
	public Node3D GetHeadBob() { return HeadBob; }
	public Node3D GetCameraSway() { return CameraSway; }
	public Node3D GetCameraCrouch() { return CameraCrouch; }
	public Node3D GetCameraLand() { return CameraLand; }
	public Node3D GetCameraJump() { return CameraJump; }
	public Node3D GetCameraShakeRot() { return CameraShakeRot; }
	public Node3D GetHeadBreathing() { return HeadBreathing; }
}
