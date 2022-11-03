using Godot;
using System;
using System.Linq;

public partial class ObjectCamera : Node3D
{
    public Node3D GimbalLand = null;
	public Node3D NodeRotY = null;
	public Node3D NodeRotX = null;
    public Node3D NodeLean = null;
	public Camera3D Camera = null;

    private Vector2 _MouseMotion = new Vector2(0f, 0f);
    private Vector2 _LookVelocity = new Vector2(0f, 0f);

    FPSCharacter_BasicMoving ownerCharacter = null;

    // lean
    Node3D LerpPos_LeanCenter = null;
    Node3D LerpPos_LeanLeft = null;
    Node3D LerpPos_LeanRight = null;
    LerpObject.LerpVector3 LerpCameraLeanPos = new LerpObject.LerpVector3();
    LerpObject.LerpFloat LerpCameraLeanRot = new LerpObject.LerpFloat();

    public enum ELeanType { Center, Left, Right };
    private ELeanType ActualLean = ELeanType.Center;
    public float LeanRotMax = 35.0f;
    public float LeanRotTarget = 0.0f;

    public override void _Ready()
	{
		NodeRotY = GetNode<Node3D>("NodeRotY");
        GimbalLand = GetNode<Node3D>("NodeRotY/GimbalLand");
        NodeRotX = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX");
        NodeLean = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean");
		Camera = GetNode<Camera3D>("NodeRotY/GimbalLand/NodeRotX/NodeLean/Camera");

        //lean
        LerpPos_LeanCenter = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/LerpPos_LeanCenter");
        LerpPos_LeanLeft = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/LerpPos_LeanLeft");
        LerpPos_LeanRight = GetNode<Node3D>("NodeRotY/GimbalLand/NodeRotX/LerpPos_LeanRight");
        
        LerpCameraLeanPos.SetAllParam(NodeLean.Position, LerpPos_LeanCenter.Position, 3.0f, true);
        //LerpCameraLeanRot.SetAllParam(NodeLean.Rotation.z, a 1.0f, true);
        
    }

    public void SetCharacterOwner(FPSCharacter_BasicMoving newFPSCharacter_BasicMoving)
    {
        ownerCharacter = newFPSCharacter_BasicMoving;
    }

	public override void _Process(double delta)
	{
        if (ownerCharacter.IsInputEnable())
            UpdateCameraLook(_MouseMotion, delta);

        //
        NodeLean.Position = LerpCameraLeanPos.Update(delta);
        
        Vector3 tempRot = NodeLean.Rotation;
        tempRot.z = Mathf.LerpAngle(tempRot.z, LeanRotTarget, 3 * (float)delta);
        NodeLean.Rotation = tempRot;
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

    public void SetActualLean(ELeanType newLeanType)
    {
        switch (newLeanType)
        {
            case ELeanType.Center:
                {
                    LerpCameraLeanPos.SetTarget(LerpPos_LeanCenter.Position);
                    LeanRotTarget = 0.0f;
                    break;
                }
            case ELeanType.Left:
                {
                    LerpCameraLeanPos.SetTarget(LerpPos_LeanLeft.Position);
                    LeanRotTarget = 0.25f;
                    break;
                }
            case ELeanType.Right:
                {
                    LerpCameraLeanPos.SetTarget(LerpPos_LeanRight.Position);
                    LeanRotTarget = -0.25f;
                    break;
                }
        }
    }

    public ELeanType GetActualLean() { return ActualLean; }
}
