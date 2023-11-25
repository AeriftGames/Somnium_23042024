using Godot;
using System;

public partial class CCharacterLookComponent : Node
{
	[Export] public Camera3D Camera;
	[Export] public float MOUSE_SENSITIVITY = 0.5f;
	[Export] public float TILT_LOWER_LIMIT = Mathf.DegToRad(-90.0f);
    [Export] public float TILT_UPPER_LIMIT = Mathf.DegToRad(90.0f);

	private FpsCharacterBase ourCharacter = null;
    private bool isMouseInput = false;
    private float rotationInput;
    private float tiltInput;
	private Vector3 mouseRotation;

	private Vector3 playerRotation;
	private Vector3 cameraRotation;

    public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public void PostInit(FpsCharacterBase newOurCharacter){ourCharacter = newOurCharacter;}

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

		playerRotation = new Vector3(0.0f, mouseRotation.Y, 0.0f);
		ourCharacter.Basis = Basis.FromEuler(playerRotation);

		cameraRotation = new Vector3(mouseRotation.X, 0.0f, 0.0f);
		Camera.Basis = Basis.FromEuler(cameraRotation);

		rotationInput = 0.0f;
		tiltInput = 0.0f;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}
