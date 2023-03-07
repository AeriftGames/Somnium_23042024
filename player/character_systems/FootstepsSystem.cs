using Godot;
using System;

public partial class FootstepsSystem : Godot.GodotObject
{
    FPSCharacter_WalkingEffects _character;

    public enum EFoot { RightFoot, LeftFoot, Stand }

    // kratky krok
    // dlouhy krok

    public FootstepsSystem(FPSCharacter_WalkingEffects newCharacter)
    {
        _character = newCharacter;
    }

    public void UpdateFootsteps(float delta)
    {
        // vypocitat vzdalenost posledniho kroku

        // testovat vzdalenost posledniho kroku a pokud je vetsi nez vzdalenost pro krok tak FootstepNow()

    }

    public void FootstepNow(EFoot newFoot)
    {
        switch (newFoot)
        {
            case EFoot.RightFoot:
                {
                    break;
                }
            case EFoot.LeftFoot:
                {
                    break;
                }
            case EFoot.Stand:
                {
                    break;
                }
        }
    }

    public string DetectGroundMaterial()
    {
        return "ground";
    }
}
