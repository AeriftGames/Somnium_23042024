using Godot;
using System;

public partial class CUniversal : Node
{
    private BlackScreen blackScreen = null;
    private LoadingHud loadingHud = null;
    private CMasterLog MasterLog = null;
    private DebugLabels debugLabels = null;

    public void PostInit()
    {
        blackScreen = GetNode<BlackScreen>("BlackScreen");

        loadingHud = GetNode<LoadingHud>("LoadingHud");
        loadingHud.Visible = false;

        MasterLog = GetNode<CMasterLog>("MasterLog");
        MasterLog.PostInit();
    }
    public LoadingHud GetLoadingHud() { return loadingHud; }
    public CMasterLog GetMasterLog() { return MasterLog; }
    // prekryje veskery hud a 3d svet cernou obrazovkou
    public void EnableBlackScreen(bool newEnable, bool new_instant = false)
    {
        blackScreen.SetActive(!newEnable, !new_instant);
    }

    public DebugLabels GetDebugLabels() {  return debugLabels; }
    public void SetDebugLabels(DebugLabels newLabels) { debugLabels = newLabels; }
}
