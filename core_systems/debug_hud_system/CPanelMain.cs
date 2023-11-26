using Godot;
using System;

public partial class CPanelMain : PanelContainer
{
    private CDebugPanel ourDebugPanel = null;

    public void PostInit(CDebugPanel newDebugPanel) 
    { 
        ourDebugPanel = newDebugPanel; 
        LoadAllMainSettings(); 
    }

    public void _on_button_quit_game_pressed() { CGameMaster.GM.QuitGame(); }
    public void _on_check_button_show_fps_toggled(bool newToggleOn) { ourDebugPanel.SetShowFps(newToggleOn); }
    public void _on_check_button_show_debug_labels_toggled(bool newToggleOn)
    {
        ourDebugPanel.GetDebugLabels().SetActive(newToggleOn);
    }
    public void _on_check_button_show_performance_toggled(bool newToggleOn)
    {
        ourDebugPanel.SetShowPerformance(newToggleOn);
    }
    public void _on_check_button_enable_occlusion_culling_toggled(bool isPressed)
    {
        Node3D worldlevel_occluderculling =
            CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene().GetLevelScene().GetLevelOcclusion();

        if (worldlevel_occluderculling != null)
        {
            worldlevel_occluderculling.Visible = isPressed;

            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO,
                "worldlevel_occluderculling set visible to: " + isPressed);
        }
        else
        {
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(this, CMasterLog.ELogMsgType.INFO,
             "worldlevel_occluderculling nebyl nalezen");
        }
    }
    public void _on_button_save_settings_pressed(){ SaveAllMainSettings(); }

    public void SaveAllMainSettings()
    {
        // ziskame ulozena data
        global_settings_data data = CGameMaster.GM.GetSettings().GetData();

        data.ShowFps = GetNode<CheckButton>("%CheckButton_ShowFps").ButtonPressed;
        data.ShowDebugLabels = GetNode<CheckButton>("%CheckButton_ShowDebugLabels").ButtonPressed;
        data.ShowPerformance = GetNode<CheckButton>("%CheckButton_ShowPerformance").ButtonPressed;
        data.EnableWorldLevelOcclusionCull = GetNode<CheckButton>("%CheckButton_EnableOcclusionCulling").ButtonPressed;

        // ulozime data
        data.Save();
    }
    public void LoadAllMainSettings() 
    {
        // ziskame ulozena data
        global_settings_data data = CGameMaster.GM.GetSettings().GetData();

        //SetEnable(data.ShowDebugHud);

        GetNode<CheckButton>("%CheckButton_ShowFps").SetPressedNoSignal(data.ShowFps);
        ourDebugPanel.SetShowFps(data.ShowFps);

        GetNode<CheckButton>("%CheckButton_ShowDebugLabels").SetPressedNoSignal(data.ShowDebugLabels);
        ourDebugPanel.GetDebugLabels().SetActive(data.ShowDebugLabels);

        GetNode<CheckButton>("%CheckButton_ShowPerformance").SetPressedNoSignal(data.ShowPerformance);
        ourDebugPanel.SetShowPerformance(data.ShowPerformance);

        GetNode<CheckButton>("%CheckButton_EnableOcclusionCulling").SetPressedNoSignal(data.EnableWorldLevelOcclusionCull);
        _on_check_button_enable_occlusion_culling_toggled(data.EnableWorldLevelOcclusionCull);
    }
}
