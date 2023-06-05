using Godot;
using System;
using System.Buffers;
using static FPSCharacter_BasicMoving;

public partial class HeadBobSystem : GodotObject
{
    InventoryObjectCamera character = null;

    public HeadBobSystem(InventoryObjectCamera ownerCharacter) 
    { 
        character = ownerCharacter;
        
    }
    public void Update(float delta)
    {

    }
    private void UpdateLandingHeadBobbing(float delta)
    {
        /*
        // Lerp pro landing pos
        HeadGimbalB.Position = HeadGimbalB.Position.Lerp(
            new Vector3(0, lerpHeadLandY, 0), lerpLandingSpeedModifier * delta);

        // Lerp pro landing rot
        objectCamera.GimbalLand.Rotation = objectCamera.GimbalLand.Rotation.Lerp(
            new Vector3(lerpHeadLandRotX, 0, 0), lerpLandingSpeedModifier * delta);*/
    }
}
