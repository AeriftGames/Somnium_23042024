using Godot;
using System;
using System.Drawing;

public partial class CharacterStairSystem : Node3D
{
    RayCast3D RaycastFrontUp;
    RayCast3D RaycastFrontDown;
    RayCast3D RaycastBack;
    RayCast3D RaycastRight;
    RayCast3D RaycastLeft;

    ShapeCast3D ShapeCast;

    bool isStairProcess;

    float MaxStepHeight = 0.35f;
    Vector3 StepOffset;

    public override void _Ready()
    {
        base._Ready();
        ShapeCast = GetNode<ShapeCast3D>("ShapeCast3D");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        /*
        Vector3 oldRot = GlobalRotation;
        oldRot.Y = GameMaster.GM.GetFPSCharacter().GetFPSCharacterCamera().GlobalRotation.Y;
        GlobalRotation = oldRot;

        if (ShapeCast.IsColliding() && GameMaster.GM.GetFPSCharacter().GetIsAnyMoveInputNow())
        {
            Node a = ShapeCast.GetCollider(0) as Node;
            if (a != null)
            {
                if (a.IsInGroup("tag_stairs"))
                {
                    //GameMaster.GM.GetFPSCharacter().ApplyFloorSnap();
                }
            }
        }
        */
    }
}
