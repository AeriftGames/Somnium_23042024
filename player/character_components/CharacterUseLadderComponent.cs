using Godot;
using System;

public partial class CharacterUseLadderComponent : Node
{
    FPSCharacter_Inventory ownCharacter = null;
    Control UseLadderControl = null;
    private bool isCanUseLadder = false;
    private ladder_system canLadderObject = null;
    public void StartInit(FPSCharacter_Inventory ownerInstance)
    {
        ownCharacter = ownerInstance;

        UseLadderControl = GetNode<Control>("UseLadderControl");
        UseLadderControl.Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (isCanUseLadder && canLadderObject != null && Input.IsActionJustPressed("UseAction"))
            canLadderObject.UseLadder(ladder_system.ELadderCharacterEffectProcess.TeleportWithBlackScreen);
    }

    public void SetCanUseLadder(bool newCanUseLadder, ladder_system newCanLadderObject)
    {
        isCanUseLadder = newCanUseLadder;
        canLadderObject = newCanLadderObject;

        if (isCanUseLadder && canLadderObject != null)
        {
            UseLadderControl.Visible = true;
        }
        else
        {
            UseLadderControl.Visible = false;
        }
    }
}
