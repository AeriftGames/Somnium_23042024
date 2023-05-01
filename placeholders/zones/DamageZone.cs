using Godot;
using System;
using System.ComponentModel.DataAnnotations;

[Tool]
public partial class DamageZone : Area3D
{
    public enum EDamageZoneType { OneShotDamage,TickDamage,DeathDamage}

    [Export] public EDamageZoneType damageZoneType = EDamageZoneType.OneShotDamage;
    [Export] public float damageValue = 10.0f;
    [Export] public float damageTickSeconds = 1.0f;
    [Export] public bool resetOnLeave = true;

    [Export] public bool _debugVisible { 
        get { return debugVisible; }
        set { debugVisible = value; SetDebugVisible(debugVisible); } }

    [Export] public bool printDebugToConsole = false;

    [Export] public Vector3 _boxSize { 
        get {return boxSize;} 
        set {boxSize = value; UpdateBoxSize(value); } }

    bool debugVisible = false;
    Vector3 boxSize = new Vector3(1,1,1);

    private bool isFinished = false;
    Godot.Timer damageTick_timer = null;

    CollisionShape3D collisionShape3D = null;
    MeshInstance3D meshInstance3D = null;

    public override void _Ready()
    {
        base._Ready();

        collisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        meshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");

        SetDebugVisible(debugVisible);

        damageTick_timer = new Godot.Timer();
        // Create timer for delaying spawn if start game
        var callable_damageTick = new Callable(this, "TickDamage");
        damageTick_timer.Connect("timeout", callable_damageTick);
        damageTick_timer.WaitTime = damageTickSeconds;
        damageTick_timer.OneShot = false;
        AddChild(damageTick_timer);
        damageTick_timer.Stop();
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void _on_body_entered(Node3D body)
    {
        // Je to hrac ?
        FPSCharacter_BasicMoving character = body as FPSCharacter_BasicMoving;
        if (character == null) return;

        if(printDebugToConsole)
            GD.Print("hrac vstoupil do DamageZone");

        switch(damageZoneType)
        {
            case EDamageZoneType.OneShotDamage:
            {
                if(!isFinished)
                {
                    //TODO DoDamage
                    isFinished = true;
                }
                break;
            }
            case EDamageZoneType.TickDamage:
            {
                StartTickDamage();
                break;
            }
            case EDamageZoneType.DeathDamage:
            {
                character.EventDead(FPSCharacter_BasicMoving.ECharacterReasonDead.NoHealth);
                break;
            }
        }
    }

    public void _on_body_exited(Node3D body)
    {
        // Je to hrac ?
        FPSCharacter_BasicMoving character = body as FPSCharacter_BasicMoving;
        if (character == null) return;

        if (printDebugToConsole)
            GD.Print("hrac odesel z DamageZone");

        switch (damageZoneType)
        {
            case EDamageZoneType.TickDamage:
            {
                damageTick_timer.Stop();
                break;
            }
        }

        if (resetOnLeave)
            ResetDamageZone();

    }
    public void ResetDamageZone()
    {
        isFinished = false;
    }

    public void StartTickDamage()
    {
        //TODO DoDamage
        damageTick_timer.Start();
    }

    public void OneTickDamage()
    {
        //TODO DoDamage
    }

    public void UpdateBoxSize(Vector3 newSize)
    {
        if (printDebugToConsole)
            GD.Print("update DamageZone box size: " + newSize);

        // zkusime cast na boxshape
        BoxShape3D boxShape = GetNode<CollisionShape3D>("CollisionShape3D").Shape as BoxShape3D;
        if (boxShape == null) return;

        BoxMesh boxMesh = GetNode<MeshInstance3D>("MeshInstance3D").Mesh as BoxMesh;
        if (boxMesh == null) return;

        boxShape.Size = newSize;
        boxMesh.Size = newSize;
    }

    public void SetDebugVisible(bool newVisible)
    {
        GetNode<MeshInstance3D>("MeshInstance3D").Visible = newVisible;
    }
}