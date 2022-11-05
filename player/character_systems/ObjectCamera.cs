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
        Vector3 finalLeanPos = TestRaycastLeansAndReturnMaxDistance(newLeanType);

        switch (newLeanType)
        {
            case ELeanType.Center:
                {
                    /*tweenLeanRot = CreateTween();
                    tweenLeanRot.TweenProperty(NodeLean, "rotation", LerpPos_LeanCenter.Rotation, tweenSpeed).SetEase(Tween.EaseType.OutIn);*/

                    tweenLeanPos = CreateTween();
                    tweenLeanPos.TweenProperty(NodeLean, "position", finalLeanPos, tweenSpeed).SetEase(Tween.EaseType.OutIn);
                    break;
                }
            case ELeanType.Left:
                {
                    /*tweenLeanRot = CreateTween();
                    tweenLeanRot.TweenProperty(NodeLean, "rotation", LerpPos_LeanLeft.Rotation, tweenSpeed).SetEase(Tween.EaseType.OutIn);*/

                    tweenLeanPos = CreateTween();
                    tweenLeanPos.TweenProperty(NodeLean, "position", finalLeanPos, tweenSpeed).SetEase(Tween.EaseType.OutIn);
                    break;
                }
            case ELeanType.Right:
                {
                    /*tweenLeanRot = CreateTween();
                    tweenLeanRot.TweenProperty(NodeLean, "rotation", LerpPos_LeanRight.Rotation, tweenSpeed).SetEase(Tween.EaseType.OutIn);*/

                    tweenLeanPos = CreateTween();
                    tweenLeanPos.TweenProperty(NodeLean, "position", finalLeanPos, tweenSpeed).SetEase(Tween.EaseType.OutIn);
                    break;
                }
        }
    }

    public ELeanType GetActualLean() { return ActualLean; }

    public Vector3 TestRaycastLeansAndReturnMaxDistance(ELeanType newLeanType)
    {
        float ray_length = 0.5f;

        UniversalFunctions.HitResult hitResult;
        hitResult.isHit = false;
        hitResult.HitPosition = Vector3.Zero;

        switch (newLeanType)
        {
            case ELeanType.Center:
                {
                    return LerpPos_LeanCenter.Position;
                }
            case ELeanType.Left:
                {
                    hitResult = UniversalFunctions.IsSimpleRaycastHit(this,
                           NodeLean.GlobalPosition,
                           NodeLean.GlobalPosition + (NodeLean.GlobalTransform.basis.x.Normalized() * -ray_length),
                           1);

                    if (hitResult.isHit)
                    {
                        GameMaster.GM.Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"hitPos: " + hitResult.HitPosition);

                        float hitLength = NodeLean.GlobalPosition.DistanceTo(hitResult.HitPosition);
                        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "raycast for lean: " + hitLength);
                        return (LerpPos_LeanCenter.Position + (NodeLean.Transform.basis.x.Normalized() * -hitLength));
                    }
                    else
                        return LerpPos_LeanLeft.Position;
                }
            case ELeanType.Right:
                {
                    hitResult = UniversalFunctions.IsSimpleRaycastHit(this,
                         NodeLean.GlobalPosition,
                         NodeLean.GlobalPosition + (NodeLean.GlobalTransform.basis.x.Normalized() * ray_length),
                         1);

                    if (hitResult.isHit)
                    {
                        GameMaster.GM.Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "hitPos: " + hitResult.HitPosition);

                        float hitLength = NodeLean.GlobalPosition.DistanceTo(hitResult.HitPosition);
                        GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "raycast for lean: " + hitLength);
                        return (LerpPos_LeanCenter.Position + (NodeLean.Transform.basis.x.Normalized() * hitLength));
                    }
                    else
                        return LerpPos_LeanRight.Position;
                }
        }
        return LerpPos_LeanLeft.Position;
    }
}
