using Godot;
using System;

public partial class CPanelMain : CPanelBase
{
    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);
    }

    public override void LoadAllElementsSettings()
    {
        base.LoadAllElementsSettings();

        // ziskame ulozena data
        global_settings_data data = CGameMaster.GM.GetSettings().GetData();

        //SetEnable(data.ShowDebugHud);

        GetNode<CheckButton>("%CheckButton_ShowFps").SetPressedNoSignal(data.ShowFps);
        GetOurDebugPanel().SetShowFps(data.ShowFps);

        GetNode<CheckButton>("%CheckButton_ShowDebugLabels").SetPressedNoSignal(data.ShowDebugLabels);
        GetOurDebugPanel().GetDebugLabels().SetActive(data.ShowDebugLabels);

        GetNode<CheckButton>("%CheckButton_ShowPerformance").SetPressedNoSignal(data.ShowPerformance);
        GetOurDebugPanel().SetShowPerformance(data.ShowPerformance);
        
        GetNode<CheckButton>("%CheckButton_EnableOcclusionCulling").SetPressedNoSignal(data.EnableWorldLevelOcclusionCull);
        _on_check_button_enable_occlusion_culling_toggled(data.EnableWorldLevelOcclusionCull);
        
    }
    public override void SaveAllElementsSettings()
    {
        base.SaveAllElementsSettings();

        // ziskame ulozena data
        global_settings_data data = CGameMaster.GM.GetSettings().GetData();

        data.ShowFps = GetNode<CheckButton>("%CheckButton_ShowFps").ButtonPressed;
        data.ShowDebugLabels = GetNode<CheckButton>("%CheckButton_ShowDebugLabels").ButtonPressed;
        data.ShowPerformance = GetNode<CheckButton>("%CheckButton_ShowPerformance").ButtonPressed;
        data.EnableWorldLevelOcclusionCull = GetNode<CheckButton>("%CheckButton_EnableOcclusionCulling").ButtonPressed;

        // ulozime data
        data.Save();
    }

    public void _on_check_button_show_fps_toggled(bool newToggleOn) { GetOurDebugPanel().SetShowFps(newToggleOn); }
    public void _on_check_button_show_debug_labels_toggled(bool newToggleOn)
    {
        GetOurDebugPanel().GetDebugLabels().SetActive(newToggleOn);
    }
    public void _on_check_button_show_performance_toggled(bool newToggleOn)
    {
        GetOurDebugPanel().SetShowPerformance(newToggleOn);
    }
    public void _on_check_button_enable_occlusion_culling_toggled(bool isPressed)
    {
        WorldLevel actualLevel = CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene();
        if (actualLevel == null) return;

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
}
