using Godot;
using System;

public partial class ObjectCamera : Node3D
{
	public Node3D NodeRotY = null;
	public Node3D NodeRotX = null;
	public Camera3D Camera = null;

    private Vector2 _MouseMotion = new Vector2(0f, 0f);
    private Vector2 _LookVelocity = new Vector2(0f, 0f);

    FPSCharacter_BasicMoving ownerCharacter = null;

    public override void _Ready()
	{
		NodeRotY = GetNode<Node3D>("NodeRotY");
        NodeRotX = GetNode<Node3D>("NodeRotY/NodeRotX");
		Camera = GetNode<Camera3D>("NodeRotY/NodeRotX/Camera");
    }

    public void SetCharacterOwner(FPSCharacter_BasicMoving newFPSCharacter_BasicMoving)
    {
        ownerCharacter = newFPSCharacter_BasicMoving;
    }

	public override void _Process(double delta)
	{
        if (ownerCharacter.IsInputEnable())
            UpdateCameraLook(_MouseMotion, delta);
    }

    // Hadle inout for mouse
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && ownerCharacter.IsInputEnable())
        {
            InputEventMouseMotion mouseEventMotion = @event as InputEventMouseMotion;
            _MouseMotion = mouseEventMotion.Relative;
        }
        else
            _MouseMotion = new Vector2(0, 0);
    }

    // Update CameraLook from mouse input and calculating rotation nodeRotY and nodeRotX
    public void UpdateCameraLook(Vector2 newMouseMotion, double delta)
    {
        // Lerping mouse motion for smooth look (x,y)
        _LookVelocity.x = Mathf.Lerp(_LookVelocity.x, newMouseMotion.x * ownerCharacter.MouseSensitivity,
            (float)delta * ownerCharacter.MouseSmooth);

        _LookVelocity.y = Mathf.Lerp(_LookVelocity.y, newMouseMotion.y * ownerCharacter.MouseSensitivity,
            (float)delta * ownerCharacter.MouseSmooth);

        // Set new rotates
        NodeRotY.RotateY(-Mathf.DegToRad(_LookVelocity.x));
        NodeRotX.RotateX(-Mathf.DegToRad(_LookVelocity.y));

        // Set clamp camera vertical look
        Vector3 actualRotX = NodeRotX.Rotation;
        actualRotX.x = Mathf.Clamp(actualRotX.x,
            Mathf.DegToRad(ownerCharacter.CameraVerticalLookMin),
            Mathf.DegToRad(ownerCharacter.CameraVerticalLookMax));

       NodeRotX.Rotation = actualRotX;

        // Reset MouseMotion
        _MouseMotion = Vector2.Zero;
    }
}
