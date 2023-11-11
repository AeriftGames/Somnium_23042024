using Godot;
using System;

public partial class CharacterStairsComponent : Node3D
{
    // 0.4 zacatek schodu
    // 0.5 konec schodu

    [Export] public bool EnableStairsDetectEffect = true;

    private FPSCharacter_Inventory inventoryCharacter = null;

    private RayCast3D rayCast3D = null;

    private float LastStairDistance = 0.0f;
    private bool FirstStep = true;

    private bool isAlreadyEndStep = true;

    public override void _Ready()
    {
        base._Ready();

        rayCast3D = GetNode<RayCast3D>("RayCast3D");
    }

    public void StartInit(FPSCharacter_Inventory ownCharacter)
    {
        inventoryCharacter = ownCharacter;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (rayCast3D.IsColliding())
        {
            Node3D hitnode = rayCast3D.GetCollider() as Node3D;
            if (hitnode == null && !GameMaster.GM.GetFPSCharacter().IsOnFloor()) return;

            float distance = Mathf.Abs(rayCast3D.GlobalPosition.DistanceTo(rayCast3D.GetCollisionPoint()));
            float rozdil = distance - LastStairDistance;
            LastStairDistance = distance;

            if (hitnode.IsInGroup("tag_stairs") && GameMaster.GM.GetFPSCharacter().IsOnFloor())
            {
                if (Mathf.Abs(rozdil) > 0.02f)
                {
                    isAlreadyEndStep = false;

                    if (rozdil > 0.02f)
                    {
                        SetMoveOnStairs(false);
                    }
                    else
                    {
                        SetMoveOnStairs(true);
                    }
                }
            }
            else
            {
                FirstStep = true;

                if (!isAlreadyEndStep)
                    EndStep(rozdil);
            }

            FirstStep = true;
        }
    }

    public void EndStep(float new_rozdil)
    {
        isAlreadyEndStep = true;

        if (GameMaster.GM.GetFPSCharacter().IsOnFloor())
        {
            if(new_rozdil > 0.02f)
            {
                GD.Print("last krok");
                //GD.Print(new_rozdil);
            }
        }
    }

    public void SetMoveOnStairs(bool newMoveOnStairsUp)
    {
        if (newMoveOnStairsUp)
        {
            GD.Print("krok nahoru");
        }
        else
        {
            GD.Print("krok dolu");
        }
    }
}
