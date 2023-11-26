using Godot;
using System;

public partial class CPanelInputs : CPanelBase
{
    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);
    }

    public override void LoadAllElementsSettings()
    {
        base.LoadAllElementsSettings();

        HSlider msmooth = GetNode<HSlider>("%mouseSmooth_HSlider");
        msmooth.Value = CGameMaster.GM.GetSettings().GetActual_LookMouseSmooth();
        Label msmooth_l = GetNode<Label>("%mouseSmooth_Label");
        msmooth_l.Text = msmooth.Value.ToString();

        HSlider msens = GetNode<HSlider>("%mouseSensitivity_HSlider");
        msens.Value = CGameMaster.GM.GetSettings().GetActual_LookMouseSensitivity();
        Label msens_l = GetNode<Label>("%mouseSensitivity_Label");
        msens_l.Text = msens.Value.ToString();

        HSlider gsens = GetNode<HSlider>("%gamepadSensitivity_HSlider");
        gsens.Value = CGameMaster.GM.GetSettings().GetActual_LookGamepadSensitivity();
        Label gsens_l = GetNode<Label>("%gamepadSensitivity_Label");
        gsens_l.Text = gsens.Value.ToString();

        HSlider gsmooth = GetNode<HSlider>("%gamepadSmooth_HSlider");
        gsmooth.Value = CGameMaster.GM.GetSettings().GetActual_LookGamepadSmooth();
        Label gsmooth_l = GetNode<Label>("%gamepadSmooth_Label");
        gsmooth_l.Text = gsmooth.Value.ToString();

        CheckButton inverselook =
            GetNode<CheckButton>("%inverseVerticalLook_CheckButton");
        inverselook.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_InverseVerticalLook();
    }

    public override void SaveAllElementsSettings()
    {
        base.SaveAllElementsSettings();
    }

    //

    public void _on_mouse_smooth_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookMouseSmooth(newValue, true, false);

        // update label
        Label label = GetNode<Label>("%mouseSmooth_Label");
        label.Text = newValue.ToString();
    }

    public void _on_mouse_sensitivity_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookMouseSensitivity(newValue, true, false);

        // update label
        Label label = GetNode<Label>("%mouseSensitivity_Label");
        label.Text = newValue.ToString();
    }

    public void _on_gamepad_smooth_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookGamepadSmooth(newValue, true, false);

        // update label
        Label label = GetNode<Label>("%gamepadSmooth_Label");
        label.Text = newValue.ToString();
    }

    public void _on_gamepad_sensitivity_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_LookGamepadSensitivity(newValue, true, false);

        // update label
        Label label = GetNode<Label>("%gamepadSensitivity_Label");
        label.Text = newValue.ToString();
    }

    public void _on_inverse_vertical_look_check_button_toggled(bool newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_InverseVerticalLook(newValue, true, false);
    }
}
