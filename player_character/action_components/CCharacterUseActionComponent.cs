using Godot;
using System;

public partial class CCharacterUseActionComponent : CBaseComponent
{
    /*
    [Export(PropertyHint.Range, "1.0,5.0,0.1")] public float DETECT_RADIUS = 3.0f;

    public struct SDetectObject { public CInteractiveObject InteractiveObject; public Vector3 HitPosition; }

    private CInteractiveObject SelectedObject = null;
    private ActionLayer actionLayer = null;
    private CHandNode handNode = null;
    
    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        GetNode<CollisionShape3D>("UseActionAreaDetect/CollisionShape3D").Shape.Set("radius", DETECT_RADIUS);
        actionLayer = GetNode<ActionLayer>("ActionLayer");
        handNode = GetNode<CHandNode>("ActionLayer/HandNode");
    }

    public void UseAction()
    {
        // mame nejaky use action object ?
        if ( GetSelectedUseActionObject() != null ) { GetSelectedUseActionObject().CallUseAction(); }
    }

    public void SetSelectedUseActionObject(CInteractiveObject newInteractiveObject) { SelectedObject = newInteractiveObject; }
    public CInteractiveObject GetSelectedUseActionObject() { return SelectedObject; }

    public void _on_use_action_area_detect_body_entered(Node3D body)
    {
        if (body.IsInGroup("interactive_object") == false) return;

        CInteractiveObject interactiveObject = body.GetParent() as CInteractiveObject;
        if (interactiveObject == null) return;
        else { interactiveObject.SetIsInRange(true); }
    }

    public void _on_use_action_area_detect_body_exited(Node3D body)
    {
        if (body.IsInGroup("interactive_object") == false) return;

        CInteractiveObject interactiveObject = body.GetParent() as CInteractiveObject;
        if (interactiveObject == null) return;
        else { interactiveObject.SetIsInRange(false); }
    }

    public void Update(double delta)
    {
        //
        if (Input.IsActionJustPressed("UseAction"))
        { UseAction(); }

        //
        SDetectObject resultDetect = DetectInteractiveObjectWithCameraRay();

        if (resultDetect.InteractiveObject != null)
        {
            // jiny objekt nez mame selektnuty ? = deselect puvodniho
            if (resultDetect.InteractiveObject != GetSelectedUseActionObject() && 
                GetSelectedUseActionObject != null)
            { TryDeselectObject(); }

            // nejaky objekt v range a v looku ? = nastavime jako selectnuty
            if (resultDetect.InteractiveObject.GetIsInRange())
            {
                SetSelectedUseActionObject(resultDetect.InteractiveObject);
                GetSelectedUseActionObject().SetIsInLook(true);

                if (GetSelectedUseActionObject().GetActionType() == CInteractiveObject.EActionType.Rect)
                    ShowBoundingBox(resultDetect.InteractiveObject, true);
                else if (GetSelectedUseActionObject().GetActionType() == CInteractiveObject.EActionType.Hand)
                    handNode.SetHandType(CHandNode.EHandType.Point);
            }
            else
            { TryDeselectObject(); }
        }
        else
        { TryDeselectObject(); }

        // ADD DEBUG LABEL
        string text = "none";
        if (SelectedObject != null)
            text = SelectedObject.GetObjectName();

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("LookActionObject",
            text,0);
    }

    public void TryDeselectObject()
    {
        if (GetSelectedUseActionObject() != null)
        {
            if (GetSelectedUseActionObject().GetActionType() == CInteractiveObject.EActionType.Rect)
                ShowBoundingBox(GetSelectedUseActionObject(), false);
            else if (GetSelectedUseActionObject().GetActionType() == CInteractiveObject.EActionType.Hand)
                handNode.SetHandType(CHandNode.EHandType.Off);

            GetSelectedUseActionObject().SetIsInLook(false);
            SetSelectedUseActionObject(null);
        }
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

    public void ShowBoundingBox(CInteractiveObject newInteractiveObject, bool newVisible)
    {
        // hide
        if (newVisible == false) 
        { 
            actionLayer.DeactiveObjectActionLayer();

            if(newInteractiveObject != null)
            {
                newInteractiveObject.GetBilboardObject().ActivateLookAtCamera(false);
            }

            return; 
        }

        // visible
        if (newInteractiveObject == null) return;

        Node3D callUseObject = newInteractiveObject.CallUseObject as Node3D;
        if(callUseObject == null) return;

        actionLayer.ActivateObjectActionLayer(newInteractiveObject);
        newInteractiveObject.GetBilboardObject().ActivateLookAtCamera(true);
    }*/
}
