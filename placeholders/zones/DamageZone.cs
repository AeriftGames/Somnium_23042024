using Godot;
using System;

public partial class DamageZone : Area3D
{
    [Export] public bool deathZone = true;

    public void _on_body_entered(Node3D body)
    {
        // Je to hrac ?
        FPSCharacter_BasicMoving character = body as FPSCharacter_BasicMoving;
        if (character == null) return;

        GD.Print("hrac vstoupil do DamageZone");

        // instant zone ?
        if (deathZone)
        {
            GD.Print("DEATH ZONE");
            character.EventDead(FPSCharacter_BasicMoving.ECharacterReasonDead.NoHealth);
        }
    }

    public void _on_body_exited(Node3D body)
    {
        // Je to hrac ?
        FPSCharacter_BasicMoving character = body as FPSCharacter_BasicMoving;
        if (character == null) return;

        GD.Print("hrac odesel z DamageZone");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}