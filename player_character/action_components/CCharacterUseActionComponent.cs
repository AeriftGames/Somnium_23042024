using Godot;
using System;

public partial class CCharacterUseActionComponent : CBaseComponent
{
    [Export(PropertyHint.Range, "1.0,5.0,0.1")] public float DETECT_RADIUS = 3.0f;

    public struct SDetectObject { public CInteractiveObject InteractiveObject; public Vector3 HitPosition; }

    private CInteractiveObject SelectedObject = null;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        GetNode<CollisionShape3D>("UseActionAreaDetect/CollisionShape3D").Shape.Set("radius", DETECT_RADIUS);
        //GD.Print("Detect radius = " + GetNode<CollisionShape3D>("UseActionAreaDetect/CollisionShape3D").Shape.Get("radius").ToString());
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
        if(resultDetect.InteractiveObject != null)
        {

            // jiny objekt nez mame selektnuty ? = deselect puvodniho
            if (resultDetect.InteractiveObject != GetSelectedUseActionObject() && 
                GetSelectedUseActionObject != null)
            {
                TryDeselectObject();
            }

            // nejaky objekt v range a v looku ? = nastavime jako selectnuty
            if (resultDetect.InteractiveObject.GetIsInRange())
            {
                SetSelectedUseActionObject(resultDetect.InteractiveObject);
                GetSelectedUseActionObject().SetIsInLook(true);
                GetSelectedUseActionObject().SetOutlineEffect(true);
            }
            else
            {
                TryDeselectObject();
            }
        }
        else
        {
            TryDeselectObject();
        }

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
            GetSelectedUseActionObject().SetIsInLook(false);
            GetSelectedUseActionObject().SetOutlineEffect(false);
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
}
