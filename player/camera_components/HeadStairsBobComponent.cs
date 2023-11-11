using Godot;
using System;

public partial class HeadStairsBobComponent : Node
{
    InventoryObjectCamera invCam = null;

    public void StartInit(InventoryObjectCamera ownerCharacter)
    {
        invCam = ownerCharacter;
    }
    public void Update(float delta)
    {
    }

    public void ApplyEffectStairStep(bool newStairUp, float newHeightValue)
    {
        Tween tween = GetTree().CreateTween();

        if (newStairUp)
        {
            tween.TweenProperty(invCam.HeadStairsBob, "position", new Vector3(0, 0.1f, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(invCam.HeadStairsBob, "position", new Vector3(0, 0.0f, 0), 0.35f).SetTrans(Tween.TransitionType.Cubic);
        }
        else
        {
            tween.TweenProperty(invCam.HeadStairsBob, "position", new Vector3(0, -0.1f, 0), 0.2f).SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(invCam.HeadStairsBob, "position", new Vector3(0, 0.0f, 0), 0.35f).SetTrans(Tween.TransitionType.Cubic);
        }
    }
}
