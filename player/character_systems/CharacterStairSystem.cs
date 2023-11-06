using Godot;
using System;

public partial class CharacterStairSystem : Node3D
{
    RayCast3D RaycastFront;
    RayCast3D RaycastBack;
    RayCast3D RaycastRight;
    RayCast3D RaycastLeft;

    bool isStairProcess;

    public override void _Ready()
    {
        base._Ready();

        RaycastFront = GetNode<RayCast3D>("RayCast3D_Front");
        RaycastBack = GetNode<RayCast3D>("RayCast3D_Back");
        RaycastRight = GetNode<RayCast3D>("RayCast3D_Right");
        RaycastLeft = GetNode<RayCast3D>("RayCast3D_Left");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Vector3 oldRot = GlobalRotation;
        oldRot.Y = GameMaster.GM.GetFPSCharacter().GetFPSCharacterCamera().GlobalRotation.Y;
        GlobalRotation = oldRot;

        RayCast3D RayCastFinal = null;
        Vector3 point;
        float distance;

        //GameMaster.GM.GetFPSCharacter().GetObjectCamera-

        if ( RayCastFinal == null && RaycastFront.IsColliding())
        {
            Node obj = RaycastFront.GetCollider() as Node;
            if ( obj != null )
            {
                if (obj.IsInGroup("tag_stair"))
                {
                    GameMaster.GM.GetFPSCharacter().LerpSpeedPosObjectCamera = 10.0f;
                    point = RaycastFront.GetCollisionPoint();
                    distance = RaycastFront.GlobalPosition.DistanceTo(point);
                    //if (distance < 0.48f || distance > 0.52f)
                    RayCastFinal = RaycastFront;
                }
                else
                {
                    GameMaster.GM.GetFPSCharacter().LerpSpeedPosObjectCamera = 15.0f;
                    return;
                }
            }
        }
        /*
        else if (RayCastFinal == null && RaycastBack.IsColliding())
        {
            point = RaycastBack.GetCollisionPoint();
            distance = RaycastBack.GlobalPosition.DistanceTo(point);
            if (distance < 0.48f || distance > 0.52f)
                RayCastFinal = RaycastBack;
        }
        else if (RayCastFinal == null && RaycastLeft.IsColliding())
        {
            point = RaycastLeft.GetCollisionPoint();
            distance = RaycastLeft.GlobalPosition.DistanceTo(point);
            if (distance < 0.48f || distance > 0.52f)
                RayCastFinal = RaycastLeft;
        }
        else if (RayCastFinal == null && RaycastRight.IsColliding())
        {
            point = RaycastRight.GetCollisionPoint();
            distance = RaycastRight.GlobalPosition.DistanceTo(point);
            if (distance < 0.48f || distance > 0.52f)
                RayCastFinal = RaycastRight;
        }
        */
        if (RayCastFinal != null)
        {
            GodotObject collider = RayCastFinal.GetCollider();
            point = RayCastFinal.GetCollisionPoint();
            Vector3 normal = RayCastFinal.GetCollisionNormal();

            distance = RayCastFinal.GlobalPosition.DistanceTo(point);
            GD.Print(distance);

            // udelat krok
            if (distance < 0.40f && !isStairProcess)
            {
                
                isStairProcess = true;
                float newDistance = GameMaster.GM.GetFPSCharacter().GlobalPosition.DistanceTo(point)/10;

                Vector3 b = GameMaster.GM.GetFPSCharacter().GlobalPosition - point;

                Vector3 a = (GameMaster.GM.GetFPSCharacter().GlobalPosition + new Vector3(0f,Mathf.Abs(b.Y),0f))
                    + GameMaster.GM.GetFPSCharacter().GlobalPosition.DirectionTo(point) * newDistance;

                GameMaster.GM.GetFPSCharacter().GlobalPosition = a;
                GameMaster.GM.GetFPSCharacter().ApplyFloorSnap();
                
                //GameMaster.GM.GetFPSCharacter().GlobalPosition = point;
            }
            else
                isStairProcess = false;
        }
    }
}

