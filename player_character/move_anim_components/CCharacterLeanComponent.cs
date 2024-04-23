using Godot;
using System;

public partial class CCharacterLeanComponent : CBaseComponent
{
    [Export] public float LeanMaxPositionDistanceX = 0.5f;
    [Export] public float LeanMaxRotateDistanceZ = 0.25f;
    [Export] public float LeanPositionTweenTime = 0.5f;
    [Export] public float LeanRaycastsTestLength = 0.8f;
    [Export] public float LeanMinCameraDistanceFromWall = 0.3f;
    [Export] public bool LeanMultiRaycastDetect = true;
    [Export] public float LeanMultiRaycastSteps = 0.15f;

    private Node3D CameraLean = null;
    Node3D LerpPos_LeanCenter = null;
    Node3D LerpPos_LeanLeft = null;
    Node3D LerpPos_LeanRight = null;

    public enum ELeanType { Center, Left, Right };
    private ELeanType ActualLean = ELeanType.Center;

    Tween tweenLeanRot = null;
    Tween tweenLeanPos = null;

    private bool waitToCenter = false;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        CameraLean = ourCharacterBase.GetNode<Node3D>("%CameraLean");
        LerpPos_LeanCenter = ourCharacterBase.GetNode<Node3D>("%LerpPos_LeanCenter");
        LerpPos_LeanLeft = ourCharacterBase.GetNode<Node3D>("%LerpPos_LeanLeft");
        LerpPos_LeanRight = ourCharacterBase.GetNode<Node3D>("%LerpPos_LeanRight");
    }

    public void InputUpdate(double delta)
    {
        if (waitToCenter)
        {
            SetActualLean(ELeanType.Center);
            if (MathF.Abs(CameraLean.Position.Length()) < 0.02f) 
            {
                waitToCenter = false;
            }
        }

        if (waitToCenter == false)
        {
            if (ourCharacterBase.GetCharacterInputState() == FpsCharacterBase.ECharacterInputState.Normal)
            {
                if (Input.IsActionPressed("leanLeft") && !Input.IsActionPressed("leanRight"))
                    SetActualLean(ELeanType.Left);
                else if (Input.IsActionPressed("leanRight") && !Input.IsActionPressed("leanLeft"))
                    SetActualLean(ELeanType.Right);
            }
            else
            {
                SetActualLean(ELeanType.Center);
            }
        }    
        
        if (Input.IsActionJustReleased("leanLeft") || Input.IsActionJustReleased("leanRight")) 
        { waitToCenter = true; }
    }

    public void SetActualLean(ELeanType newLeanType)
    {

        if (tweenLeanPos != null)
            tweenLeanPos.Kill();

        if (tweenLeanRot != null)
            tweenLeanRot.Kill();

        //GD.Print(newLeanType);

        // vypocet nove pozice pro aktualni leaning
        Vector3 finalLeanPos = CalculateLeanPositionWithRaycasts(newLeanType,
            LeanRaycastsTestLength, LeanMaxPositionDistanceX,LeanMinCameraDistanceFromWall,
            LeanMultiRaycastDetect,LeanMultiRaycastSteps);

        // vypocet nove rotace
        Vector3 finalLeanRot = CalculateLeanRotation(newLeanType, finalLeanPos,
           LeanMaxPositionDistanceX, LeanMaxRotateDistanceZ);

        // Rot
        tweenLeanRot = CreateTween().SetParallel(true);
        tweenLeanRot.Parallel().TweenProperty(CameraLean, "rotation", finalLeanRot,
            LeanPositionTweenTime).SetEase(Tween.EaseType.OutIn).SetTrans(Tween.TransitionType.Cubic);

        // Pos
        tweenLeanRot.Parallel().TweenProperty(CameraLean, "position", finalLeanPos,
            LeanPositionTweenTime).SetEase(Tween.EaseType.OutIn).SetTrans(Tween.TransitionType.Cubic);
    }

    public ELeanType GetActualLean() { return ActualLean; }

    private Vector3 CalculateLeanPositionWithRaycasts(ELeanType newLeanType, float newRayLength,
        float newLeanMaxPositionX, float newLeanMinDistanceFromWall, bool multiRaycast, float multiRaycastStep)
    {
        // provede raycasty detekci kolize v pozadovanem smeru pro vyklon,
        // pokud nejaky kolizni bod je nastavujeme podle vzdalenosti (center <-> collision_point) novou pozici,
        // ktera prakticky snizuje puvodni maximalni rozsah leaningu.

        // pokud zadny kolizni bod neni, pouzijeme v pozadovanem smeru plny rozsah leaningu
        // ve finale tedy vracime vypocitany novy lokalni bod(pozici) leaningu

        // multiraycast je zalozeny na 3 raycastech z predniho, prostredniho (stejny jako v singleRaycast) a zadniho

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

        // detect by single raycast
        if (multiRaycast == false)
        {
            // 1. main raycast
            UniversalFunctions.HitResult hitResult = UniversalFunctions.IsSimpleRaycastHit(ourCharacterBase,
                CameraLean.GlobalPosition,
                CameraLean.GlobalPosition +
                CameraLean.GlobalTransform.Basis.X.Normalized() * (ray_length * direction_x), 1);

            if (hitResult.isHit)
            {
                float hitLength = CameraLean.GlobalPosition.DistanceTo(hitResult.HitPosition) -
                leanMinCameraDistanceFromWall;

                if (hitLength < leanMaxPositionX)
                {
                    returnedVector = LerpPos_LeanCenter.Position +
                        (CameraLean.Transform.Basis.Y.Normalized() * (hitLength * direction_x));
                }
            }
        }
        else
        {
            // detect by multiraycast

            // 1. main raycast
            UniversalFunctions.HitResult hitResult = UniversalFunctions.IsSimpleRaycastHit(ourCharacterBase,
                CameraLean.GlobalPosition,
                CameraLean.GlobalPosition +
                CameraLean.GlobalTransform.Basis.X.Normalized() * (ray_length * direction_x), 1);

            // 2. predni raycast
            UniversalFunctions.HitResult hit2Result = UniversalFunctions.IsSimpleRaycastHit(ourCharacterBase,
                CameraLean.GlobalPosition + (CameraLean.GlobalTransform.Basis.Z.Normalized() * -multiRaycastStep),
                CameraLean.GlobalPosition + (CameraLean.GlobalTransform.Basis.Z.Normalized() * -multiRaycastStep) +
                CameraLean.GlobalTransform.Basis.X.Normalized() * ((ray_length) * direction_x), 1);

            // 3. zadni raycast
            UniversalFunctions.HitResult hit3Result = UniversalFunctions.IsSimpleRaycastHit(ourCharacterBase,
                CameraLean.GlobalPosition + (CameraLean.GlobalTransform.Basis.Z.Normalized() * multiRaycastStep),
                CameraLean.GlobalPosition + (CameraLean.GlobalTransform.Basis.Z.Normalized() * multiRaycastStep) +
                CameraLean.GlobalTransform.Basis.X.Normalized() * ((ray_length) * direction_x), 1);

            if (hitResult.isHit || hit2Result.isHit || hit3Result.isHit)
            {
                float hitLength = CameraLean.GlobalPosition.DistanceTo(hitResult.HitPosition) -
                    leanMinCameraDistanceFromWall;

                float hit2Length = CameraLean.GlobalPosition.DistanceTo(hit2Result.HitPosition) -
                    leanMinCameraDistanceFromWall;

                float hit3Length = CameraLean.GlobalPosition.DistanceTo(hit3Result.HitPosition) -
                    leanMinCameraDistanceFromWall;

                float nejmensi = hitLength;
                if (hitLength < hit2Length && hitLength < hit3Length)
                {
                    //hitLength je nejmensi
                    nejmensi = hitLength;
                }
                else if (hit2Length < hitLength && hit2Length < hit3Length)
                {
                    //hit2Length je nejmensi
                    nejmensi = hit2Length;
                }
                else if (hit3Length < hitLength && hit3Length < hit2Length)
                {
                    //hit3Length je nejmensi
                    nejmensi = hit3Length;
                }

                if (nejmensi < leanMaxPositionX)
                {
                    CGameMaster.GM.GetDebugHud().CustomLabelUpdateText(4, this, "raycast for lean: " + nejmensi);

                    returnedVector = LerpPos_LeanCenter.Position +
                        (CameraLean.Transform.Basis.X.Normalized() * (nejmensi * direction_x));
                }
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
                    returnedVector.Z = newMaxLeanRotZ;
                    direction_x = 1;
                    break;
                }
            case ELeanType.Right:
                {
                    returnedVector.Z = -newMaxLeanRotZ;
                    direction_x = -1;
                    break;
                }
        }

        // aktualni distance lean
        float testDistance = newMaxLeanPosX - (newMaxLeanPosX - Mathf.Abs(newActualLeanPos.X));

        //percentage rozsah lean pos
        float percentage_pos = testDistance / newMaxLeanPosX * 100f;

        //percentage rozsah lean rot
        float percentage_rot_step = newMaxLeanRotZ / 100f;
        float per_rot_final = (percentage_pos * percentage_rot_step) * direction_x;

        // nastavime finalni vector
        returnedVector.Z = per_rot_final;

        return returnedVector;
    }
}
