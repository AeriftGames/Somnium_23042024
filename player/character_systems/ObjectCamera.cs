using Godot;
using System;
using System.Linq;
using static Godot.TextServer;

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

    public enum ELeanType { Center, Left, Right };
    private ELeanType ActualLean = ELeanType.Center;

    Tween tweenLeanRot = null;
    Tween tweenLeanPos = null;

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
        // ziskame dostupnost funkci(WalkingEffects) z naseho zakladniho charactera
        FPSCharacter_WalkingEffects characterWalkingEffects = (FPSCharacter_WalkingEffects)ownerCharacter;
        if (characterWalkingEffects == null) return;

        // vypocet nove pozice pro aktualni leaning
        Vector3 finalLeanPos = CalculateLeanPositionWithRaycasts(newLeanType, 
            characterWalkingEffects.LeanRaycastsTestLength,characterWalkingEffects.LeanMaxPositionDistanceX,
            characterWalkingEffects.LeanMinCameraDistanceFromWall);

        // vypocet nove rotace
        Vector3 finalLeanRot = CalculateLeanRotation(newLeanType,finalLeanPos,
            characterWalkingEffects.LeanMaxPositionDistanceX,characterWalkingEffects.LeanMaxRotateDistanceZ);

        // Rot
        tweenLeanRot = CreateTween();
        tweenLeanRot.TweenProperty(NodeLean, "rotation", finalLeanRot,
            characterWalkingEffects.LeanPositionTweenTime).SetEase(Tween.EaseType.OutIn);
        
        // Pos
        tweenLeanPos = CreateTween();
        tweenLeanPos.TweenProperty(NodeLean, "position", finalLeanPos,
            characterWalkingEffects.LeanPositionTweenTime).SetEase(Tween.EaseType.OutIn);
    
    }

    public ELeanType GetActualLean() { return ActualLean; }

    private Vector3 CalculateLeanPositionWithRaycasts(ELeanType newLeanType,float newRayLength,
        float newLeanMaxPositionX,float newLeanMinDistanceFromWall)
    {
        // provede raycasty detekci kolize v pozadovanem smeru pro vyklon,
        // pokud nejaky kolizni bod je nastavujeme podle vzdalenosti (center <-> collision_point) novou pozici,
        // ktera prakticky snizuje puvodni maximalni rozsah leaningu.

        // pokud zadny kolizni bod neni, pouzijeme v pozadovanem smeru plny rozsah leaningu
        // ve finale tedy vracime vypocitany novy lokalni bod(pozici) leaningu

        Vector3 returnedVector = Vector3.Zero;
        float direction_x = 0;
        float ray_length = newRayLength;
        float leanMaxPositionX = newLeanMaxPositionX;
        float leanMinCameraDistanceFromWall = newLeanMinDistanceFromWall;

        switch (newLeanType)
        {
            case ELeanType.Center:
                {
                    returnedVector = Vector3.Zero;
                    return returnedVector;
                }
            case ELeanType.Left:
                {
                    // raycast smer po ose x doleva
                    direction_x = -1;
                    returnedVector = new Vector3(leanMaxPositionX * direction_x, 0, 0);
                    break;
                }
            case ELeanType.Right:
                {
                    // raycast smer po ose x doprava
                    direction_x = 1;
                    returnedVector = new Vector3(leanMaxPositionX * direction_x, 0, 0);
                    break;
                }
        }

        UniversalFunctions.HitResult hitResult = UniversalFunctions.IsSimpleRaycastHit(this,
            NodeRotX.GlobalPosition,
            NodeRotX.GlobalPosition + 
            NodeLean.GlobalTransform.basis.x.Normalized() * (ray_length * direction_x), 1);

            if (hitResult.isHit)
            {
                float hitLength = NodeRotX.GlobalPosition.DistanceTo(hitResult.HitPosition) -
                    leanMinCameraDistanceFromWall;

                if(hitLength < leanMaxPositionX)
                {
                    GameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "raycast for lean: " + hitLength);

                    returnedVector = LerpPos_LeanCenter.Position +
                        (NodeRotX.Transform.basis.x.Normalized() * (hitLength * direction_x));
                }
            }

        return returnedVector;
    }

    private Vector3 CalculateLeanRotation(ELeanType newLeanType, Vector3 newActualLeanPos,
        float newMaxLeanPosX, float newMaxLeanRotZ)
    {
        // Vypocet lean rotace pomoci procentualniho rozsahu pozice z nehoz vypocteme procenualni rozsahu rotace
        // na konc vracime novy vector jako novou lokalni rotaci leaningu

        Vector3 returnedVector = Vector3.Zero;
        float direction_x = 0.0f;

        switch (newLeanType)
        {
            case ELeanType.Center:
                {
                    returnedVector = Vector3.Zero;
                    return returnedVector;
                }
            case ELeanType.Left:
                {
                    returnedVector.z = newMaxLeanRotZ;
                    direction_x = 1;
                    break;
                }
            case ELeanType.Right:
                {
                    returnedVector.z = -newMaxLeanRotZ;
                    direction_x = -1;
                    break;
                }
        }

        // aktualni distance lean
        float testDistance = newMaxLeanPosX - (newMaxLeanPosX - Mathf.Abs(newActualLeanPos.x));

        //percentage rozsah lean pos
        float percentage_pos = testDistance / newMaxLeanPosX * 100f;
        
        //percentage rozsah lean rot
        float percentage_rot_step = newMaxLeanRotZ / 100f;
        float per_rot_final = (percentage_pos * percentage_rot_step) * direction_x;

        // nastavime finalni vector
        returnedVector.z = per_rot_final;

        return returnedVector;
    }
}
