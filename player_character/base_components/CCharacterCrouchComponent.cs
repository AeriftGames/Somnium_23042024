using Godot;
using System;

public partial class CCharacterCrouchComponent : Node
{
    [Export(PropertyHint.Range, "1,5,0.1")] public float CROUCH_SPEED = 2.0f;

    private FpsCharacterBase ourCharacter = null;
    private AnimationPlayer animPlayerCrouch;
    private ShapeCast3D shapeCastUncrouch;

    private bool isCrouching = false;

    public override void _Ready()
	{
        animPlayerCrouch = GetNode<AnimationPlayer>("AnimationPlayerCrouch");
        shapeCastUncrouch = GetNode<ShapeCast3D>("ShapeCastUncrouch");
	}

    public void PostInit(FpsCharacterBase newOurCharacter)
    {
        ourCharacter = newOurCharacter;

        shapeCastUncrouch.AddException(ourCharacter);
    }
    public void ToggleCrouch(){SetCrouch(!isCrouching);}
    public void SetCrouch(bool newCrouch)
    {
        if (newCrouch)
        {
            animPlayerCrouch.Play("Crouch", -1, CROUCH_SPEED);
            isCrouching = newCrouch;
        }
        else if(shapeCastUncrouch.IsColliding() == false)
        {
            animPlayerCrouch.Play("Crouch", -1, -CROUCH_SPEED,true);
            isCrouching = newCrouch;
        }
    }

}
