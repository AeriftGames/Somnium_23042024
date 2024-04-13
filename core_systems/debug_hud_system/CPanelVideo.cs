using Godot;
using System;

public partial class CPanelVideo : CPanelBase
{
    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);

        //ApplyAllVideoControls();
    }

    public override void LoadAllElementsSettings()
    {
        base.LoadAllElementsSettings();

        ApplyAllVideoControls();
    }

    public override void SaveAllElementsSettings()
    { 
        base.SaveAllElementsSettings();

        CGameMaster.GM.GetSettings().SaveActual_AllGraphicsSettings();
    }

    //

    public void CheckScreenModeSetting()
    {
        // pokud mame vybrany mod windowed, tak povolime moznost vybrat velikost okna, jinak ne
        if (CGameMaster.GM.GetSettings().GetActual_ScreenMode() == 0)
        {
            GetNode<OptionButton>("%WindowSize_OptionButton").Disabled = false;
            GetNode<OptionButton>("%WindowSize_OptionButton").Selected = CGameMaster.GM.GetSettings().GetActual_ScreenSizeID();
        }
        else
        {
            GetNode<OptionButton>("%WindowSize_OptionButton").Disabled = true;
            GetNode<OptionButton>("%WindowSize_OptionButton").Selected = 2;
        }
    }
    public void _on_screen_mode_option_button_item_selected(int newID)
    {
        CGameMaster.GM.GetSettings().Apply_ScreenMode(newID, true, false);
        CheckScreenModeSetting();   // volame pro logiku zapnuti/vypnuti moznosti vybirat velikost okna
    }
    public void _on_window_size_option_button_item_selected(int newID)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_ScreenSizeID(newID, true, false);
    }
    public void _on_antialias_option_button_item_selected(int newID)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_AntialiasID(newID, true, false);
    }
    public void _on_gi_option_button_item_selected(int newID)
    {
        //only apply
        CGameMaster.GM.GetSettings().Apply_GlobalIlumination(newID, true, false);
    }
    public void _on_scale_3d_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_Scale3D(newValue / 100.0f, true, false);

        Label scale3dLabel = GetNode<Label>("%Scale3dvalue_Label");
        scale3dLabel.Text = (newValue / 100.0f).ToString();
    }
    public void _on_half_res_gi_check_box_toggled(bool newPressed)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_HalfResolutionGI(newPressed, true, false);
    }
    public void _on_ssao_check_box_toggled(bool newPressed)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_Ssao(newPressed, true, false);
    }
    public void _on_ssil_check_box_toggled(bool newPressed)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_Ssil(newPressed, true, false);
    }
    public void _on_unlock_max_fps_check_box_toggled(bool newPressed)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_UnlockMaxFps(newPressed, true, false);
    }
    public void _on_disable_vsync_check_box_toggled(bool newPressed)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_DisableVsync(newPressed, true, false);
    }

    public void ApplyAllVideoControls()
    {
        // screen mode
        OptionButton screenmode_option = GetNode<OptionButton>("%ScreenMode_OptionButton");
        screenmode_option.Selected = CGameMaster.GM.GetSettings().GetActual_ScreenMode();

        CheckScreenModeSetting();   // volame pro logiku zapnuti/vypnuti moznosti vybirat velikost okna

        // antialias 
        OptionButton antialias_option = GetNode<OptionButton>("%Antialias_OptionButton");
        antialias_option.Selected = CGameMaster.GM.GetSettings().GetActual_AntialiasID();

        OptionButton windowsize_option = GetNode<OptionButton>("%WindowSize_OptionButton");
        windowsize_option.Selected = CGameMaster.GM.GetSettings().GetActual_ScreenSizeID();

        // gi
        OptionButton gi_option = GetNode<OptionButton>("%GI_OptionButton");
        gi_option.Selected = CGameMaster.GM.GetSettings().GetActual_GlobalIlumination();

        // scale 3d
        HSlider scale3d_slider = GetNode<HSlider>("%Scale3d_HSlider");
        scale3d_slider.Value = CGameMaster.GM.GetSettings().GetActual_Scale3D() * 100.0f;

        Label scale3d_label = GetNode<Label>("%Scale3dvalue_Label");
        scale3d_label.Text = CGameMaster.GM.GetSettings().GetActual_Scale3D().ToString();

        // half resolution gi
        CheckBox halfresgi_checkbox = GetNode<CheckBox>("%HalfResGI_CheckBox");
        halfresgi_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_HalfResolutionGI();

        // ssao
        CheckBox ssao_checkbox = GetNode<CheckBox>("%Ssao_CheckBox");
        ssao_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_Ssao();

        // ssil
        CheckBox ssil_checkbox = GetNode<CheckBox>("%Ssil_CheckBox");
        ssil_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_Ssil();

        CheckBox unlockmaxfps_checkbox = GetNode<CheckBox>("%UnlockMaxFps_CheckBox");
        unlockmaxfps_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_UnlockMaxFps();

        CheckBox vsync_checkbox = GetNode<CheckBox>("%DisableVsync_CheckBox");
        vsync_checkbox.ButtonPressed = CGameMaster.GM.GetSettings().GetActual_DisableVsync();
    }
}
