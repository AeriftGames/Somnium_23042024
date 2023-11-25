using Godot;
using System;

public partial class ball_projectile : RigidBody3D
{
    public enum EShootBallActionType { RigidBody, SplashDecal }
    EShootBallActionType ShootBallActionType = EShootBallActionType.RigidBody;

    public void SetActionType(EShootBallActionType newShootBallActionType)
    {
        ShootBallActionType = newShootBallActionType;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public void _on_body_entered(Node body)
    {
        GD.Print("contact");

        if (ShootBallActionType == EShootBallActionType.SplashDecal)
            CreateSplashDecal();
    }

    public void CreateSplashDecal()
    {
        PackedScene SplashPrefab = GD.Load<PackedScene>("res://testing_stuff_kaen/shootball/SplashDecal.tscn");
        Node3D SplashNode = SplashPrefab.Instantiate() as Node3D;

        CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene().AddChild(SplashNode);
        SplashNode.GlobalPosition = GlobalPosition;
        SplashNode.LookAtFromPosition(SplashNode.GlobalPosition, CGameMaster.GM.GetGame().GetFPSCharacter().GlobalPosition);

        QueueFree();
    }
}
