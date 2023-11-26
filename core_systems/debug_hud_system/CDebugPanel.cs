using Godot;

public partial class CDebugPanel : Control
{
    private bool isOpen = false;
    private Timer TimerUpdate = new Timer();

    private Label Label_FPS;
    private ColorRect Background = null;
    private MarginContainer MarginPerformance = null;
    private CDebugLabels DebugLabels = null;
    private MarginContainer DebugTabs = null;

    private CPanelMainNew PanelMain = null;

    public void PostInit()
    {
        Label_FPS = GetNode<Label>("%Label_FPS");
        Background = GetNode<ColorRect>("%Background");
        MarginPerformance = GetNode<MarginContainer>("%MarginPerformance");
        DebugLabels = GetNode<CDebugLabels>("%DebugLabels");
        DebugTabs = GetNode<MarginContainer>("%DebugTabs");

        TimerUpdate.Timeout += TimerUpdateTick;
        TimerUpdate.WaitTime = 0.2;
        TimerUpdate.OneShot = false;
        AddChild(TimerUpdate);
        TimerUpdate.Start();

        Background.Visible = false;
        DebugTabs.Visible = false;

        // Tabs
        PanelMain = GetNode<CPanelMainNew>("%PanelMain");
        PanelMain.PostInit(this);

    }

    public void ToggleOpen() { OpenDebugTabs(!isOpen); }
    public void OpenDebugTabs(bool newOpen)
    {
        // pokud je in game menu otevrene - ignorujeme timhle akci otevrit debug hud
        if (CGameMaster.GM.GetGame().GetInGameMenu().GetIsOpen()) return;

        isOpen = newOpen;

        if (isOpen)
        {
            DebugTabs.Visible = true;
            Background.Visible = true;

            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            DebugTabs.Visible = false;
            Background.Visible = false;

            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public bool GetIsOpen() { return isOpen; }

    public void TimerUpdateTick()
    {
        if (Label_FPS.Visible)
            Label_FPS.Text = Engine.GetFramesPerSecond().ToString();

        if (MarginPerformance.Visible)
        {
            GetNode<Label>("%Label_DrawCalls").Text = Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame).ToString();
            GetNode<Label>("%Label_Objects").Text = Performance.GetMonitor(Performance.Monitor.RenderTotalObjectsInFrame).ToString();
        }
    }

    public void SetShowFps(bool newShow) { GetNode<MarginContainer>("%MarginFps").Visible = newShow; }
    public bool GetShowFps() { return GetNode<MarginContainer>("%MarginFps").Visible; }
    public void SetShowPerformance(bool newShow) { MarginPerformance.Visible = newShow; }
    public bool GetShowPerformance() { return MarginPerformance.Visible; }
    public CDebugLabels GetDebugLabels() { return DebugLabels; }
}
