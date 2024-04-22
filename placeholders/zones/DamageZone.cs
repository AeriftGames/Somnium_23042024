using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

[Tool]
public partial class DamageZone : Area3D
{
    public enum EDamageZoneType { OneShotDamage, TickDamage, DeathDamage, OneAddHealth, TickHealth}

    [Export] public EDamageZoneType damageZoneType = EDamageZoneType.OneShotDamage;
    [Export] public float damageValue = 10.0f;
    [Export] public float damageTickSeconds = 1.0f;
    [Export] public bool resetOnLeave = true;

    private FPSCharacterAction characterInZone = null;

    [Export] public bool _debugVisible {
        get { return debugVisible; }
        set { debugVisible = value; SetDebugVisible(debugVisible); } }

    [Export] public Color _debugBoxColor { 
        get { return debugBoxColor; } 
        set {debugBoxColor = value; SetDebugMeshColor(value); } }
    [Export] public bool printDebugToConsole = false;

    [Export] public Vector3 _boxSize { 
        get {return boxSize;} 
        set {boxSize = value; UpdateBoxSize(value); } }

    bool debugVisible = false;
    Color debugBoxColor = Color.Color8(255, 0, 0,100);
    Vector3 boxSize = new Vector3(1,1,1);

    private bool isFinished = false;
    Godot.Timer damageTick_timer = null;
    Godot.Timer healthTick_timer = null;

    CollisionShape3D collisionShape3D = null;
    MeshInstance3D meshInstance3D = null;

    private bool enableStart = false;

    public override async void _Ready()
    {
        base._Ready();

        collisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");
        meshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");

        SetDebugVisible(debugVisible);

        // Create timer for delaying spawn if start game
        damageTick_timer = new Godot.Timer();
        var callable_damageTick = new Callable(this, "OneTickDamage");
        damageTick_timer.Connect("timeout", callable_damageTick);
        damageTick_timer.WaitTime = damageTickSeconds;
        damageTick_timer.OneShot = false;
        AddChild(damageTick_timer);
        damageTick_timer.Stop();

        // Create timer for delaying spawn if start game
        healthTick_timer = new Godot.Timer();
        var callable_healthTick = new Callable(this, "OneTickHealth");
        healthTick_timer.Connect("timeout", callable_healthTick);
        healthTick_timer.WaitTime = damageTickSeconds;
        healthTick_timer.OneShot = false;
        AddChild(healthTick_timer);
        healthTick_timer.Stop();

        DelayStart(2000);
    }

    public async void DelayStart(int MSDelay)
    {
        await Task.Delay(MSDelay);
        enableStart = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void _on_body_entered(Node3D body)
    {
        if (enableStart == false) return;

        // Je to hrac ?
        characterInZone = body as FPSCharacterAction;
        if (characterInZone == null) return;

        if(printDebugToConsole)
            GD.Print("character vstoupil do DamageZone");

        switch(damageZoneType)
        {
            case EDamageZoneType.OneShotDamage:
            {
                if(!isFinished)
                {
                    OneTickDamage();
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
                if (printDebugToConsole)
                    GD.Print("Death Damage");

                characterInZone.GetHealthComponent().ApplyDamage(
                    characterInZone.GetHealthComponent().GetHealthMath().GetMaxHealth()*10);
                break;
            }
            case EDamageZoneType.OneAddHealth:
            {
                if(!isFinished)
                {
                    OneTickHealth();
                    isFinished = true;
                }
                break;
            }
            case EDamageZoneType.TickHealth:
            {
                StartTickHealth();
                break;
            }
        }
    }

    public void _on_body_exited(Node3D body)
    {
        if (enableStart == false) return;

        // Je to hrac ?
        characterInZone = body as FPSCharacterAction;
        if (characterInZone == null) return;

        if (printDebugToConsole)
            GD.Print("character odesel z DamageZone");

        switch (damageZoneType)
        {
            case EDamageZoneType.TickDamage:
            {
                if (printDebugToConsole)
                    GD.Print("Stop Tick Damage");

                damageTick_timer.Stop();
                break;
            }
            case EDamageZoneType.TickHealth:
            {
                if (printDebugToConsole)
                    GD.Print("Stop Tick Health");

                healthTick_timer.Stop();
                break;
            }
        }

        if (resetOnLeave)
            ResetDamageZone();

    }
    public void ResetDamageZone()
    {
        if (printDebugToConsole)
            GD.Print("Reset Damage Zone");

        isFinished = false;
    }

    public void StartTickDamage()
    {
        if (printDebugToConsole)
            GD.Print("Start Tick Damage");

        OneTickDamage();    // prvni damage, pak uz podle timeru
        damageTick_timer.Start();
    }

    public void StartTickHealth()
    {
        if (printDebugToConsole)
            GD.Print("Start Tick Damage");

        OneTickHealth();    // prvni damage, pak uz podle timeru
        healthTick_timer.Start();
    }

    public void OneTickDamage()
    {
        if (characterInZone == null) return;

        if (printDebugToConsole)
            GD.Print("One Tick Damage");

        characterInZone.GetHealthComponent().ApplyDamage(damageValue);
    }

    public void OneTickHealth()
    {
        if (characterInZone == null) return;

        if (printDebugToConsole)
            GD.Print("One Tick Health");

        characterInZone.GetHealthComponent().ApplyHeal(damageValue);
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

    public void SetDebugMeshColor(Color color)
    {
        StandardMaterial3D mat = GetNode<MeshInstance3D>("MeshInstance3D").GetActiveMaterial(0) as StandardMaterial3D;
        if (mat == null) return;
        mat.AlbedoColor = color;
    }
}