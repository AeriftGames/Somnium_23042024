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

    Tween tweenLeanRot = null;
    Tween tweenLeanPos = null;
    float tweenSpeed = 0.5f;

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
    }

    public void SetCharacterOwner(FPSCharacter_BasicMoving newFPSCharacter_BasicMoving)
    {
        ownerCharacter = newFPSCharacter_BasicMoving;
    }

	public override void _Process(double delta)
	{
        if (ownerCharacter.IsInputEnable())
            UpdateCameraLook(_MouseMotion, delta);

        RaycastTestForLean();
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
                    tweenLeanRot = CreateTween();
                    tweenLeanRot.TweenProperty(NodeLean, "rotation", LerpPos_LeanCenter.Rotation, tweenSpeed).SetEase(Tween.EaseType.OutIn);

                    tweenLeanPos = CreateTween();
                    tweenLeanPos.TweenProperty(NodeLean, "position", LerpPos_LeanCenter.Position, tweenSpeed).SetEase(Tween.EaseType.OutIn);
                    break;
                }
            case ELeanType.Left:
                {
                    tweenLeanRot = CreateTween();
                    tweenLeanRot.TweenProperty(NodeLean, "rotation", LerpPos_LeanLeft.Rotation, tweenSpeed).SetEase(Tween.EaseType.OutIn);

                    tweenLeanPos = CreateTween();
                    tweenLeanPos.TweenProperty(NodeLean, "position", LerpPos_LeanLeft.Position, tweenSpeed).SetEase(Tween.EaseType.OutIn);
                    break;
                }
            case ELeanType.Right:
                {
                    tweenLeanRot = CreateTween();
                    tweenLeanRot.TweenProperty(NodeLean, "rotation", LerpPos_LeanRight.Rotation, tweenSpeed).SetEase(Tween.EaseType.OutIn);

                    tweenLeanPos = CreateTween();
                    tweenLeanPos.TweenProperty(NodeLean, "position", LerpPos_LeanRight.Position, tweenSpeed).SetEase(Tween.EaseType.OutIn);
                    break;
                }
        }
    }

    public ELeanType GetActualLean() { return ActualLean; }

    public void RaycastTestForLean()
    {
        float ray_length = 0.5f;

        // do jedne strany
        UniversalFunctions.HitResult leftRayHit1 = UniversalFunctions.IsSimpleRaycastHit(this,
            NodeLean.GlobalPosition,
            (NodeLean.GlobalPosition + new Vector3(0, 0, 0.0f)) -
            NodeRotX.GlobalTransform.basis.x.Normalized() * ray_length);

        // do druhe strany
        UniversalFunctions.HitResult rightRayHit1 = UniversalFunctions.IsSimpleRaycastHit(this,
            NodeLean.GlobalPosition,
            (NodeLean.GlobalPosition + new Vector3(0, 0, 0.0f)) +
            NodeRotX.GlobalTransform.basis.x.Normalized() * ray_length);


        //
        bool left;
        if (leftRayHit1.isHit /*|| leftRayHit2.isHit || leftRayHit3.isHit*/)
            left = true;
        else
            left = false;

        bool right;
        if (rightRayHit1.isHit /*|| rightRayHit2.isHit || rightRayHit3.isHit*/)
            right = true;
        else
            right = false;

        if(left)
        {
            float hit_length = NodeLean.GlobalPosition.DistanceTo(leftRayHit1.HitPosition);
            GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "leftRayHit: " + hit_length);
            GD.Print(leftRayHit1.HitNode.Name);
        }
        else
        {
            GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "leftRayHit: 0 ");
        }

        if(right)
        {
            float hit_length = NodeLean.GlobalPosition.DistanceTo(rightRayHit1.HitPosition);
            GameMaster.GM.GetDebugHud().CustomLabelUpdateText(5, this, "rightRayHit: " + hit_length);
            GD.Print(rightRayHit1.HitNode.Name);
        }
        else
        {
            GameMaster.GM.GetDebugHud().CustomLabelUpdateText(5, this, "rightRayHit: 0 ");
        }
        /*
        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "leftRayHit: " + left);
        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(5, this, "rightRayHit: " + right);*/
    }
}
