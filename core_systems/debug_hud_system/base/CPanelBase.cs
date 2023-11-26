using Godot;
using System;

public partial class CPanelBase : PanelContainer
{
    private CDebugPanel ourDebugPanel = null;

    private VBoxContainer VBoxElements = null;
    private VBoxContainer VBoxBaseButtons = null;

    public virtual void PostInit(CDebugPanel newDebugPanel) 
    { 
        ourDebugPanel = newDebugPanel; 
        LoadAllElementsSettings(); 
    }

    public CDebugPanel GetOurDebugPanel() { return ourDebugPanel; }

    public VBoxContainer GetVBoxElements() { return VBoxElements; }
    public VBoxContainer GetVBoxBaseButtons() { return VBoxBaseButtons; }

    public void _on_button_save_settings_pressed() { SaveAllElementsSettings(); }
    public void _on_button_quit_game_pressed() { CGameMaster.GM.QuitGame(); }
    public virtual void SaveAllElementsSettings() { /*for override*/ }
    public virtual void LoadAllElementsSettings() { /* for override*/ }
}
