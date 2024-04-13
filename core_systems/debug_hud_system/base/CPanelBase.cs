using Godot;
using System;

public partial class CPanelBase : PanelContainer
{
    [Export] public Control FirstElementInBox = null;

    private CDebugPanel ourDebugPanel = null;

    private VBoxContainer VBoxElements = null;
    private VBoxContainer VBoxBaseButtons = null;
    public bool isFocus = false;

    public virtual void PostInit(CDebugPanel newDebugPanel) 
    { 
        ourDebugPanel = newDebugPanel;

        VBoxElements = GetNode<VBoxContainer>("%VBoxElements");
        VBoxBaseButtons = GetNode<VBoxContainer>("%VBoxButtons");
    }

    public CDebugPanel GetOurDebugPanel() { return ourDebugPanel; }

    public VBoxContainer GetVBoxElements() { return VBoxElements; }
    public VBoxContainer GetVBoxBaseButtons() { return VBoxBaseButtons; }

    public void _on_button_save_settings_pressed() { SaveAllElementsSettings(); }
    public void _on_button_quit_game_pressed() { CGameMaster.GM.QuitGame(); }
    public virtual void SaveAllElementsSettings() { /*for override*/ }
    public virtual void LoadAllElementsSettings() { /* for override*/ }

    public Control GetFirstElementInVbox() 
    { 
        if (FirstElementInBox == null) { GD.Print("GetFirstElementInVBox return null, please check CPanelBase"); return null;}
        else {  return FirstElementInBox; }
    }

    public void FocusFirstElement()
    {
        if(FirstElementInBox != null)
        {
            FirstElementInBox.GrabFocus();
            isFocus = true;
        }
    }

    public void Defocus()
    {
        FirstElementInBox.ReleaseFocus();
        isFocus = false;
    }

    public void SetOurAnchor()
    {
        SetAnchorsPreset(LayoutPreset.TopLeft);
    }
}
