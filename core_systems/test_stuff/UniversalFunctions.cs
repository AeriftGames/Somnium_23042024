using Godot;
using System;
using System.IO;

public partial class UniversalFunctions
{
    public struct HitResult
    {
        public bool isHit;
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public Node HitNode;
    }

    static public HitResult IsSimpleRaycastHit(Node3D owner, Vector3 from, Vector3 to, uint collisionMask)
    {
        HitResult result = new HitResult();

        PhysicsDirectSpaceState3D directSpace = owner.GetWorld3D().DirectSpaceState;
        if(directSpace != null)
        {
            PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
            rayParam.From = from;
            rayParam.To = to;
            rayParam.CollisionMask = collisionMask;

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
    public static string[] GetDirectoryFiles(string directory, string filetype)
    {
        string[] a = Directory.GetFiles(Directory.GetCurrentDirectory() + directory, "*" + filetype);
        if (a == null) return null;
        if (a.Length == 0) return null;
        
        return a;
    }

    public static string GetStringBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }
}