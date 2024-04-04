using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class CCharacterInteractionComponent : CBaseComponent
{
    public struct SDetectObject { public CInteractiveObject InteractiveObject; public Vector3 HitPosition; }

    private CInteractiveObject SelectedObject = null;
    private CHandNode HandNode = null;

    RigidBody3D pickedBody = null;
    bool isGrabbing = false;
    bool isCanNewGrab = true;
    bool wantRotateNow = false;

    bool canUse = true;
    //only for GrabActions
    bool isFirstGrabAction = true;
    Vector2 originalHandGrabbedTex = Vector2.Zero;
    Vector3 hit_offset = Vector3.Zero;

    public struct SPhysicalGrabbedItemParams
    {
        public Vector3 inertia;
        public float angularDamp;
        public float linearDamp;
        public float friction;
        public float bounce;
        public float mass;
    }

    SPhysicalGrabbedItemParams lastGrabbedItemOriginalParams;

    //
    [Export(PropertyHint.Range, "1.0,5.0,0.1")] public float DETECT_RADIUS = 3.0f;

    [Export] public bool CanUse = true;
    [Export] public bool CanGrabObject = true;
    [Export] public bool CanThrowObject = true;
    [Export] public bool CanRotateObject = true;
    [Export] public bool CanMoveFarOrNearObject = true;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        GetNode<CollisionShape3D>("UseActionAreaDetect/CollisionShape3D").Shape.Set("radius", DETECT_RADIUS);
        HandNode = GetNode<CHandNode>("ActionLayer/HandNode");
    }
    public void _on_use_action_area_detect_body_entered(Node3D newBody)
    {
        if (newBody.IsInGroup("interactive_object") == false) return;

        CInteractiveObject interactiveObject = newBody.GetParent() as CInteractiveObject;
        if (interactiveObject == null) return;
        else { interactiveObject.SetIsInRange(true); }
    }
    public void _on_use_action_area_detect_body_exited(Node3D newBody)
    {
        if (newBody.IsInGroup("interactive_object") == false) return;

        CInteractiveObject interactiveObject = newBody.GetParent() as CInteractiveObject;
        if (interactiveObject == null) return;
        else { interactiveObject.SetIsInRange(false); }
    }
    public SDetectObject DetectInteractiveObjectWithCameraRay()
    {
        SDetectObject returnDetect = new SDetectObject();
        returnDetect.HitPosition = Vector3.Zero;
        returnDetect.InteractiveObject = null;

        PhysicsDirectSpaceState3D directSpace = ourCharacterBase.GetWorld3D().DirectSpaceState;
        if (directSpace == null) return returnDetect;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = ourCharacterBase.GetCharacterLookComponent().GetMainCamera().GlobalPosition;
        rayParam.To = ourCharacterBase.GetCharacterLookComponent().GetMainCamera().GlobalPosition -
            ourCharacterBase.GetCharacterLookComponent().GetMainCamera().GlobalTransform.Basis.Z * 10;

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
        {
            Node HitCollider = (Node)rayResult["collider"];
            if (HitCollider == null) return returnDetect;
            if (HitCollider.GetParent() == null) return returnDetect;

            if (HitCollider.GetParent().IsInGroup("interactive_object"))
            {
                returnDetect.HitPosition = (Vector3)rayResult["position"];
                returnDetect.InteractiveObject = (CInteractiveObject)HitCollider.GetParent();
            }
        }

        return returnDetect;
    }
    public void SetSelectedUseActionObject(CInteractiveObject newInteractiveObject) { SelectedObject = newInteractiveObject; }
    public CInteractiveObject GetSelectedUseActionObject() { return SelectedObject; }

    public bool IsInputEnable() 
    { 
        if (ourCharacterBase.GetCharacterInputState() == FpsCharacterBase.ECharacterInputState.Normal) return true;
        else return false;
    }

    public void DeselectHand() 
    { 
        HandNode.SetHandType(CHandNode.EHandType.Off);

        if(SelectedObject != null)
        {
            SelectedObject.SetIsInLook(false);
            SelectedObject = null;
        }
    }

    public void PhysicUpdate(double delta)
    {
        bool useNow = false, grabNow = false, throwObjectNow = false,
            rotateGrabbedObject = false, moveFarGrabbedObject = false, moveNearGrabbedObject = false;

        useNow = IsInputEnable() && CanUse && Input.IsActionJustPressed("UseAction");
        grabNow = IsInputEnable() && CanGrabObject && Input.IsActionPressed("mouseClickLeft");
        throwObjectNow = IsInputEnable() && CanThrowObject && Input.IsActionJustPressed("throwObject");
        rotateGrabbedObject = IsInputEnable() && CanRotateObject && Input.IsActionPressed("rotateGrabbedObject");
        moveFarGrabbedObject = IsInputEnable() && CanMoveFarOrNearObject && Input.IsActionJustReleased("moveFarGrabbedObject");
        moveNearGrabbedObject = IsInputEnable() && CanMoveFarOrNearObject && Input.IsActionJustReleased("moveNearGrabbedObject");

        SDetectObject resultDetect = DetectInteractiveObjectWithCameraRay();

        if (resultDetect.InteractiveObject != null)
        {
            if (resultDetect.InteractiveObject.GetIsInRange())
            {
                SetSelectedUseActionObject(resultDetect.InteractiveObject);
                SelectedObject.SetIsInLook(true);
                switch (SelectedObject.InteractiveLevel)
                {
                    case CInteractiveObject.EInteractiveLevel.OnlyUse:
                        {
                            UpdateForUse(SelectedObject, useNow, delta);
                            break;
                        }
                    case CInteractiveObject.EInteractiveLevel.OnlyPhysic:
                        {
                            UpdateForPhysic(SelectedObject, useNow, grabNow, delta);
                            break;
                        }
                    case CInteractiveObject.EInteractiveLevel.UseAndPhysic:
                        {
                            UpdateForUse(SelectedObject, useNow, delta);
                            UpdateForPhysic(SelectedObject, useNow, grabNow, delta);
                            break;
                        }
                }
            }
            else
                DeselectHand();
        }
        else
            DeselectHand();
    }

    public void UpdateForUse(CInteractiveObject newInteractiveObject,bool newUseNow,double newDelta)
    {
        //  Pokud mame pouzivame objekt pro USE - zobrazime text pro jeho akci
        if (newInteractiveObject.InteractiveLevel == CInteractiveObject.EInteractiveLevel.OnlyUse ||
            newInteractiveObject.InteractiveLevel == CInteractiveObject.EInteractiveLevel.UseAndPhysic)
        {
            switch (newInteractiveObject.InteractVisibleBy)
            {
                case CInteractiveObject.EUseInteractVisibleBy.Text:
                    {
                        /*
                        basicHud.SetUseLabelText(newInteractiveObject.GetUseActionName());
                        basicHud.SetUseVisible(true);*/
                        break;
                    }
                case CInteractiveObject.EUseInteractVisibleBy.HandClick:
                    {
                        /*basicHud.SetHandClickVisible(true);*/
                        HandNode.SetHandType(CHandNode.EHandType.Point);
                        break;
                    }
                case CInteractiveObject.EUseInteractVisibleBy.HandClickAndText:
                    {
                        /*
                        basicHud.SetUseLabelText(newInteractiveObject.GetUseActionName());
                        basicHud.SetUseVisible(true);
                        basicHud.SetHandClickVisible(true);*/
                        break;
                    }
                default:
                    break;
            }

            // Pokud je momentalne od hrace input zadost pro USE, pouzijeme vnitrni funkci objektu pro USE
            if (newUseNow && CanUse)
            {
                if (newInteractiveObject != null)
                {
                    newInteractiveObject.CallUseAction();
                }
            }
        }
    }

    public void UpdateForPhysic(CInteractiveObject newInteractiveObject,bool newUseNow,bool newGrabNow,double newDelta)
    {

    }

    public void StopGrabbing()
    {
        /*
        basicHud.SetHandGrabState(false, false);
        basicHud.SetCrosshairVisible(true);

        SetRigidBodyParamForGrab(pickedBody, false);
        isGrabbing = false;
        character.objectCamera.HandGrabJoint.NodeB = character.objectCamera.HandGrabJoint.GetPath();

        // Set to original handGrabPosition
        Vector3 actualPosition = character.objectCamera.GetHandGrabMarker().Position;
        actualPosition.Z = -character.MoveFarOrNearObjectOriginal;
        character.objectCamera.GetHandGrabMarker().Position = actualPosition;*/
    }
}
