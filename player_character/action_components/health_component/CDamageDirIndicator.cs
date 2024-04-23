using Godot;
using System;

public partial class CDamageDirIndicator : Control
{
    public enum EDamageDir { Right, Left, Up, Down, Center };

    FPSCharacterAction CharAction = null;

    private AnimationPlayer AnimationPlayer_DamageDir = null;
    public void PostInit()
    {
        CharAction = CGameMaster.GM.GetGame().GetFPSCharacterBase() as FPSCharacterAction;
        AnimationPlayer_DamageDir = GetNode<AnimationPlayer>("AnimationPlayer_DamageDir");
    }

    public void ApplyDamageDirEffect(EDamageDir newDamageDir)
    {
        switch (newDamageDir)
        {
            case EDamageDir.Right:
                AnimationPlayer_DamageDir.Play("DamageRight");
                break;
            case EDamageDir.Left:
                AnimationPlayer_DamageDir.Play("DamageLeft");
                break;
            case EDamageDir.Up:
                AnimationPlayer_DamageDir.Play("DamageUp");
                break;
            case EDamageDir.Down:
                AnimationPlayer_DamageDir.Play("DamageDown");
                break;
            case EDamageDir.Center:
                AnimationPlayer_DamageDir.Play("DamageCenter");
                break;
        }
    }
}
