using Godot;
using System;
using System.Threading.Tasks;

public partial class CCharacterShootBallComponent : CBaseComponent
{
    [ExportGroupAttribute("ShootProjectile")]
    [Export] bool CanShootProjectile = true;

    [Export]
    ball_projectile.EShootBallActionType ShootBallActionType =
        ball_projectile.EShootBallActionType.RigidBody;

    [Export] float PowerImpulseShoot = 10.0f;
    [Export] float MassProjectile = 5.0f;
    [Export] bool EnableAutoDestroyProjectile = true;
    [Export] int MSecToDestroyProjectile = 5000;
    [Export] bool CanPlaySoundEffect = true;
    [Export] bool CanDoCamShake = true;

    AudioStreamPlayer AudioStreamPlayer_ShootBall;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        AudioStreamPlayer_ShootBall = GetNode<AudioStreamPlayer>("AudioStreamPlayer_ShootBall");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // strelba
        bool shootNow = ourCharacterBase.GetCharacterInputState() 
            == FpsCharacterBase.ECharacterInputState.Normal && 
            CanShootProjectile && Input.IsActionJustPressed("mouseRightClick");

        if (shootNow)
            ShootPhysicProjectile();
    }

    public void ShootPhysicProjectile()
    {
        // smer strileni
        Vector3 start = ourCharacterBase.GetCharacterLookComponent().
            GetMainCamera().GlobalPosition - new Vector3(0, 0.2f, 0);

        Vector3 end = ourCharacterBase.GetCharacterLookComponent().GetMainCameraLookingPointPos();

        // vytvoreni projektilu - ball
        ball_projectile projectile =
            GD.Load<PackedScene>("res://testing_stuff_kaen/shootball/ball_projectile.tscn")
            .Instantiate() as ball_projectile;

        CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene().AddChild(projectile);

        projectile.SetActionType(ShootBallActionType);
        projectile.Mass = MassProjectile;
        projectile.GlobalPosition = start;
        projectile.ApplyCentralImpulse((end - start) * PowerImpulseShoot);

        // zvuk
        if(CanPlaySoundEffect)
            AudioStreamPlayer_ShootBall.Play();
        
        // shake
        FPSCharacterAction actionChar = ourCharacterBase as FPSCharacterAction;
        if (actionChar != null && CanDoCamShake) 
            actionChar.GetCharacterCameraShakeComponent().ApplyUserParamShake(1.0f, 20.0f);

        // auto destroy - pouze kdyz je action type RigidBody
        if (EnableAutoDestroyProjectile && ShootBallActionType == ball_projectile.EShootBallActionType.RigidBody)
            DestroyProjectile(projectile);
    }

    public async void DestroyProjectile(RigidBody3D projectile)
    {
        await Task.Delay(MSecToDestroyProjectile);
        if (projectile != null)
            projectile.QueueFree();
    }
}
