using Godot;
using System;

public partial class CGame : Node
{
    private FPSCharacter_BasicMoving fPSCharacter_BasicMoving_Instance = null;
    private CLevelLoader LevelLoader = null;
    private CInGameMenu InGameMenu = null;
    private CDebugHud DebugHud = null;
    private CDebugPanel DebugPanel = null;

    public void SetFPSCharacter(FPSCharacter_BasicMoving newFpsCharater) { fPSCharacter_BasicMoving_Instance = newFpsCharater; }
    public FPSCharacter_BasicMoving GetFPSCharacter() { return fPSCharacter_BasicMoving_Instance; }
    public CLevelLoader GetLevelLoader() { return LevelLoader; }
    public CInGameMenu GetInGameMenu() { return InGameMenu; }
    public CDebugPanel GetDebugPanel() { return DebugPanel; }

    private bool isInGameMenuOpen = false;

    public void PostInit()
    {
        LevelLoader = GetNode<CLevelLoader>("LevelLoader");
        LevelLoader.PostInit(false);

        InGameMenu = GetNode<CInGameMenu>("InGameMenu");
        InGameMenu.PostInit();

        DebugHud = GetNode<CDebugHud>("DebugHud");
        DebugHud.PostInit();

        DebugPanel = GetNode<CDebugPanel>("DebugPanel");
        DebugPanel.PostInit();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("EscapeAction"))
            GetInGameMenu().ToggleOpen();

        if (Input.IsActionJustPressed("ToggleDebugHud"))
            GetDebugPanel().ToggleOpen();
    }
}
