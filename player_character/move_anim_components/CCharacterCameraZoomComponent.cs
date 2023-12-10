using Godot;
using System;

public partial class CCharacterCameraZoomComponent : CBaseComponent
{
    [Export] public float FOV_OFFSET_NORMAL = 0.0f;
    [Export] public float FOV_OFFSET_ZOOM = -20.0f;
    [Export] public float FOV_LERPSPEED = 4.0f;

    // Zoom
    private float neededZoomValue;
    private LerpObject.LerpFloat LerpObject_CameraZoom = new LerpObject.LerpFloat();
    private Vector3 lookingPoint = Vector3.Zero;

    private float WorkFov = 0.0f;

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        if (EnableComponent == false) return;

        //Zoom
        LerpObject_CameraZoom.EnableUpdate(true);
        LerpObject_CameraZoom.SetTarget(FOV_OFFSET_NORMAL);     // Initial setup = normal fov
        lookingPoint = ourCharacterBase.GetCharacterLookComponent().GetMainCameraLookingPointPos();
    }

    public void InputUpdate(double delta)
    {
        if (EnableComponent == false) return;

        // Camera Zoom
        if (Input.IsActionPressed("CameraZoom"))
            SetZoom(true);
        else if (Input.IsActionJustReleased("CameraZoom"))
            SetZoom(false);
    }

    public void Update(double delta)
    {
        if (EnableComponent == false) return;

        // CameraZoom Process
        if (Mathf.Abs(LerpObject_CameraZoom.GetTarget() - WorkFov) > 0.15f)
        {
            WorkFov = LerpObject_CameraZoom.ActualUpdate(WorkFov, delta);

            // finalni nastaveni offset fov
            ourCharacterBase.GetCharacterLookComponent().SetFovOffset("Zoom",WorkFov);
        }
    }

    public void SetZoom(bool newZoom, float newZoomValue = -1.0f, float newZoomInterpSpeed = -1.0f)
    {
        if (EnableComponent == false) return;

        // true = zoomed, false = zoom to normal value
        if (newZoom)
        {
            // zoom value
            if (newZoomValue == -1.0f)
                LerpObject_CameraZoom.SetTarget(FOV_OFFSET_ZOOM);
            else
                LerpObject_CameraZoom.SetTarget(newZoomValue);

            // speed value
            if (newZoomInterpSpeed == -1)
                LerpObject_CameraZoom.SetSpeed(FOV_LERPSPEED);
            else
                LerpObject_CameraZoom.SetSpeed(newZoomInterpSpeed);
        }
        else
        {
            // zoom to normal - vzdy k normal hodnote a rychlosti
            LerpObject_CameraZoom.SetTarget(FOV_OFFSET_NORMAL);

            // speed value
            if (newZoomInterpSpeed == -1)
                LerpObject_CameraZoom.SetSpeed(FOV_LERPSPEED);
            else
                LerpObject_CameraZoom.SetSpeed(newZoomInterpSpeed);
        }
    }
}