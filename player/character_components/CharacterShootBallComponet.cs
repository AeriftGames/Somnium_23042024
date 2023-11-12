using Godot;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

public partial class CharacterShootBallComponet : Node
{
    [ExportGroupAttribute("ShootProjectile")]
    [Export] bool CanShootProjectile = true;
    [Export] float PowerImpulseShoot = 10.0f;
    [Export] float MassProjectile = 5.0f;
    [Export] bool EnableAutoDestroyProjectile = true;
    [Export] int MSecToDestroyProjectile = 5000;

    AudioStreamPlayer AudioStreamPlayer_ShootBall;

    FPSCharacter_Inventory ownCharacter = null;

    public void StartInit(FPSCharacter_Inventory ownerInstance)
    {
        ownCharacter = ownerInstance;

        AudioStreamPlayer_ShootBall = GetNode<AudioStreamPlayer>("AudioStreamPlayer_ShootBall");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        bool shootNow = ownCharacter.IsInputEnable() && CanShootProjectile && Input.IsActionJustPressed("mouseRightClick");
        if (shootNow)
            ShootPhysicProjectile();
    }

    public void ShootPhysicProjectile()
    {
        Vector3 start = ownCharacter.GetObjectCamera().GlobalPosition - new Vector3(0,0.2f,0);
        Vector3 end = ownCharacter.GetObjectCamera().GetCameraLookingPoint().GlobalPosition;

        RigidBody3D projectile =
            GD.Load<PackedScene>("res://testing_stuff_kaen/shootball/ball_projectile.tscn").Instantiate() as RigidBody3D;

        GameMaster.GM.GetLevelLoader().GetActualLevelScene().AddChild(projectile);
        projectile.Mass = MassProjectile;
        projectile.GlobalPosition = start;
        projectile.ApplyCentralImpulse((end - start) * PowerImpulseShoot);

        AudioStreamPlayer_ShootBall.Play();

        if (EnableAutoDestroyProjectile)
            DestroyProjectile(projectile);
    }

    public async void DestroyProjectile(RigidBody3D projectile)
    {
        await Task.Delay(MSecToDestroyProjectile);
        if (projectile != null)
            projectile.QueueFree();
    }
}
