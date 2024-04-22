using Godot;
using System;

public partial class CCharacterFovComponent : CBaseComponent
{
    [Export] public float FOV_NORMAL = 70.0f;
    [Export] public float FOV_WALKRUN_INTERPSPEED = 1.5f;
    [Export] public bool FOV_WALK_ENABLE = true;
    [Export] public float FOV_WALK_NEEDVALUE = 4.0f;
    [Export] public bool FOV_RUNNING_ENABLE = true;
    [Export] public float FOV_RUNNING_NEEDVALUE = 12.0f;

    [Export] public bool FOV_JUMP_ENABLE = true;
    [Export] public float FOV_JUMP_NEEDVALUE = 4.0f;
    [Export] public bool FOV_LAND_ENABLE = true;
    public float FOV_LAND_NEEDVALUE = 0.0f;
    [Export] public bool FOV_CROUCH_ENABLE = true;
    [Export] public float FOV_CROUCH_NEEDVALUE = 3.0f;

    // FOV offsets
    private float finalOffset = 0.0f;
    private float breathOffset = 0.0f;
    private float zoomOffset = 0.0f;
    private float walkrunOffset = 0.0f;
    private float jumpOffset = 0.0f;
    private float landOffset = 0.0f;
    private float crouchOffset = 0.0f;

    public void Update(double delta)
    {
        if (EnableComponent == false) return;

        UpdateMovementFoV(delta);
        ApplyFinalFov();
    }

    public void SetFovOffset(string newFovOffsetName, float newOffsetValue)
    {
        if (newFovOffsetName == "Breath") { breathOffset = newOffsetValue; }
        else if (newFovOffsetName == "Zoom") { zoomOffset = newOffsetValue; }
        else if (newFovOffsetName == "WalkRun") { walkrunOffset = newOffsetValue; }
        else if (newFovOffsetName == "Jump") { jumpOffset = newOffsetValue; }
        else if (newFovOffsetName == "Land") { landOffset = newOffsetValue; }
        else if (newFovOffsetName == "Crouch") { crouchOffset = newOffsetValue; }
    }

    public void ApplyFinalFov()
    {
        float finalFov = FOV_NORMAL + breathOffset + zoomOffset +
        walkrunOffset + jumpOffset + landOffset + crouchOffset;

        if (ourCharacterBase.GetCharacterLookComponent() == null) return;
            ourCharacterBase.GetCharacterLookComponent().GetMainCamera().Fov = finalFov;

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Final Camera Fov",
            float.Round(ourCharacterBase.GetCharacterLookComponent().GetMainCamera().Fov, 1).ToString(), 4);
    }

    public void UpdateMovementFoV(double delta)
    {
        // walk and run fov
        if (FOV_WALK_ENABLE &&
            ourCharacterBase.GetCharacterMovementComponent().GetWantSpeed() ==
            ourCharacterBase.GetCharacterMovementComponent().SPEED_WALK &&
            ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 0.2f)
        { walkrunOffset = Mathf.Lerp(walkrunOffset, FOV_WALK_NEEDVALUE, (float)delta * FOV_WALKRUN_INTERPSPEED); }
        if (FOV_RUNNING_ENABLE &&
            ourCharacterBase.GetCharacterMovementComponent().GetWantSpeed() ==
            ourCharacterBase.GetCharacterMovementComponent().SPEED_SPRINT &&
            ourCharacterBase.GetCharacterMovementComponent().GetRealSpeed() > 3.0f)
        { walkrunOffset = Mathf.Lerp(walkrunOffset, FOV_RUNNING_NEEDVALUE, (float)delta * FOV_WALKRUN_INTERPSPEED); }
        else
        { walkrunOffset = Mathf.Lerp(walkrunOffset, 0.0f, (float)delta * FOV_WALKRUN_INTERPSPEED); }

        SetFovOffset("WalkRun", walkrunOffset);

        // jump fov
        if (FOV_JUMP_ENABLE &&
            ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "JumpPlayerState")
        { jumpOffset = Mathf.Lerp(jumpOffset, FOV_JUMP_NEEDVALUE, (float)delta * 2.0f); }
        else
        { jumpOffset = Mathf.Lerp(jumpOffset, 0.0f, (float)delta * 2.0f); }

        SetFovOffset("Jump", jumpOffset);

        // land fov = controlled from tween in SetLandFovNow function
        if (FOV_LAND_ENABLE)
        { landOffset = Mathf.Lerp(landOffset, FOV_LAND_NEEDVALUE, (float)delta * 50.0f); }

        SetFovOffset("Land", landOffset);

        // crouch fov
        if (FOV_CROUCH_ENABLE &&
            ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "IdleCrouchPlayerState" ||
            ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "CrouchMovePlayerState")
        { crouchOffset = Mathf.Lerp(crouchOffset, FOV_CROUCH_NEEDVALUE, (float)delta * 3.5f); }
        else { crouchOffset = Mathf.Lerp(crouchOffset, 0.0f, (float)delta * 3.5f); }

        SetFovOffset("Crouch", crouchOffset);
    }

    public void SetLandFovNow()
    {
        Tween tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Quad);
        tween.TweenProperty(this, "FOV_LAND_NEEDVALUE", -2.0f, 0.35f);
        tween.TweenProperty(this, "FOV_LAND_NEEDVALUE", 0.0f, 1.0f);
    }
}
