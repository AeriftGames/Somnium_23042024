using Godot;
using static Godot.TextServer;

public partial class CCharacterLookComponent : CBaseComponent
{
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
	private Node3D HeadFocusAction = null;
	private Node3D HeadBreathing = null;
	private Camera3D Camera = null;
	private Node3D LookingPoint = null;
	private Node3D RightHandPoint = null;

	private Node3D HeadForwardNode = null;
	private Node3D SpawnItemPoint = null;

	private Vector2 LookGamepad = Vector2.Zero;


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
		HeadFocusAction = ourCharacterBase.GetNode<Node3D>("%HeadFocusAction");
		HeadBreathing = ourCharacterBase.GetNode<Node3D>("%HeadBreathing");
		Camera = ourCharacterBase.GetNode<Camera3D>("%Camera3D");
		LookingPoint = GetMainCamera().GetNode<Node3D>("LookPoint");
		RightHandPoint = GetMainCamera().GetNode<Node3D>("RightHandPoint");

		HeadForwardNode = GetNode<Node3D>("%HeadForwardNode");
		SpawnItemPoint = GetNode<Node3D>("%SpawnItemPoint");


		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		isMouseInput = @event is InputEventMouseMotion eventMouseMotion_test && Input.MouseMode == Input.MouseModeEnum.Captured;

		// Prisel event pohybu mysi ? provedeme kalkulaci
		if (isMouseInput) 
		{ CalculateByMouse(@event); }
	}

	public void CalculateByMouse(InputEvent @event)
	{
        if (ourCharacterBase.GetCharacterInputState() == FpsCharacterBase.ECharacterInputState.Normal)
        {
            InputEventMouseMotion eventMouseMotion = @event as InputEventMouseMotion;
            rotationInput = -eventMouseMotion.Relative.X * MOUSE_SENSITIVITY;
            tiltInput = -eventMouseMotion.Relative.Y * MOUSE_SENSITIVITY;
        }
        else if (ourCharacterBase.GetCharacterInputState() == FpsCharacterBase.ECharacterInputState.DontMoveAndLook)
        {
            rotationInput = 0.0f;
            tiltInput = 0.0f;
        }
    }

	public void CalculateByGamepad(double delta)
	{
		Vector2 InputDir = new Vector2(
			Input.GetActionStrength("LookRight") - Input.GetActionStrength("LookLeft"),
			Input.GetActionStrength("LookDown") - Input.GetActionStrength("LookUp"));//.LimitLength(1.0f);

        LookGamepad = LookGamepad.Lerp(new Vector2(InputDir.X, InputDir.Y), (float)delta * 20.0f);

		// prisel event pohybu mysi ? preskocime nasledujici nastaveni rotationInput a tiltInput z Gamepadu
		// musi tak byt kvuli vynulovani hodnot pro mys a gamepad - protoze mys funguje na eventu
		if (isMouseInput) return;
		
        rotationInput = -LookGamepad.X * MOUSE_SENSITIVITY * 8;
        tiltInput = -LookGamepad.Y * MOUSE_SENSITIVITY * 8;
        
    }

	public void UpdateFinalLook(double delta)
	{
		CalculateByGamepad(delta);

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
	public Node3D GetHeadFocusAction() { return HeadFocusAction; }

	public Node3D GetHeadForwardNode() {  return HeadForwardNode; }
	public Node3D GetSpawnItemPoint() {  return SpawnItemPoint; }
}
