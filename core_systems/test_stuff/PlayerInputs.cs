using Godot;
using System;

public partial class PlayerInputs
{
    public static Vector2 GetRightStickMotion(float newSpeedModifier = 1.0f, bool newDebugPrint = false)
    {
        // ziskani motion vectoru z packy gamepadu
        Vector2 stickMotion = new Vector2(Input.GetActionStrength("RightStick_Right") - Input.GetActionStrength("RightStick_Left"),
            -(Input.GetActionStrength("RightStick_Up") - Input.GetActionStrength("RightStick_Down")));

        // aplikovani speed modifieru
        stickMotion = stickMotion * newSpeedModifier;

        if (newDebugPrint)
            GD.Print("(Right Stick) Motion: " + stickMotion);

        return stickMotion;
    }
    public static Vector2 GetLeftStickMotion(float newSpeedModifier = 1.0f, bool newDebugPrint = false)
    {
        // ziskani motion vectoru z packy gamepadu
        Vector2 stickMotion = new Vector2(Input.GetActionStrength("LeftStick_Right") - Input.GetActionStrength("LeftStick_Left"),
            -(Input.GetActionStrength("LeftStick_Up") - Input.GetActionStrength("LeftStick_Down")));

        // aplikovani speed modifieru
        stickMotion = stickMotion * newSpeedModifier;

        if (newDebugPrint)
            GD.Print("(Left Stick) Motion: " + stickMotion);

        return stickMotion;
    }
}
