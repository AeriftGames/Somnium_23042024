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

    private CPanelMain PanelMain = null;
    private CPanelLevels PanelLevels = null;
    private CPanelSpawns PanelSpawns = null;
    private CPanelVideo PanelVideo = null;
    private CPanelAudio PanelAudio = null;
    private CPanelInputs PanelInputs = null;

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
        PanelMain = GetNode<CPanelMain>("%PanelMain");
        PanelMain.PostInit(this);

        PanelLevels = GetNode<CPanelLevels>("%PanelLevels");
        PanelLevels.PostInit(this);

        PanelSpawns = GetNode<CPanelSpawns>("%PanelSpawns");
        PanelSpawns.PostInit(this);

        PanelVideo = GetNode<CPanelVideo>("%PanelVideo");
        PanelVideo.PostInit(this);

        PanelAudio = GetNode<CPanelAudio>("%PanelAudio");
        PanelAudio.PostInit(this);

        PanelInputs = GetNode<CPanelInputs>("%PanelInputs");
        PanelInputs.PostInit(this);

    }

    public void ToggleOpen() { OpenDebugTabs(!isOpen); }
    public void OpenDebugTabs(bool newOpen)
    {
        // pokud je in game menu otevrene - ignorujeme timhle akci otevrit debug hud
        if (CGameMaster.GM.GetGame().GetInGameMenu().GetIsOpen()) return;

        isOpen = newOpen;

        if (isOpen)
        {
            // for old fps character open
            if (CGameMaster.GM.GetGame().GetFPSCharacterOld() != null)
            {
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetInputEnable(false);
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetMouseVisible(true);
            }

            DebugTabs.Visible = true;
            Background.Visible = true;

            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            // for old fps character close
            FPSCharacter_Inventory charInventory = CGameMaster.GM.GetGame().GetFPSCharacterOld() as FPSCharacter_Inventory;
            if (charInventory != null)
            {
                if (charInventory.GetInventoryMenu().GetActive()) return;

                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetInputEnable(true);
                CGameMaster.GM.GetGame().GetFPSCharacterOld().SetMouseVisible(false);
            }

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

    public void AllLoadSettings()
    {
        PanelMain.LoadAllElementsSettings();
        PanelVideo.LoadAllElementsSettings();
        PanelAudio.LoadAllElementsSettings();
        PanelInputs.LoadAllElementsSettings();
    }

    public CPanelSpawns GetPanelSpawns() { return PanelSpawns; }
}
