using Godot;
using System;

public partial class UniversalFunctions
{
    public struct HitResult
    {
        public bool isHit;
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public Node HitNode;
    }

    static public HitResult IsSimpleRaycastHit(Node3D owner, Vector3 from, Vector3 to)
    {
        HitResult result = new HitResult();

        PhysicsDirectSpaceState3D directSpace = owner.GetWorld3d().DirectSpaceState;
        if(directSpace != null)
        {
            PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
            rayParam.From = from;
            rayParam.To = to;

            var rayResult = directSpace.IntersectRay(rayParam);
            if (rayResult.Count > 0)
            {
                result.isHit = true;
                result.HitPosition = (Vector3)rayResult["position"];
                result.HitNormal = (Vector3)rayResult["normal"];
                result.HitNode = (Node)rayResult["collider"];
            }
            else
            {
                result.isHit = false;
                result.HitPosition = Vector3.Zero;
                result.HitNormal = Vector3.Zero;
                result.HitNode = null;
            }
        }

        return result;
    }
}