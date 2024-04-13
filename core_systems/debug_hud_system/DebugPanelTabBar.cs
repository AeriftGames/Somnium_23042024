using Godot;
using System;

public partial class DebugPanelTabBar : TabBar
{
    [Export] public CPanelBase ourPanel;

    public void _on_focus_entered()
    {
        if (ourPanel != null)
            ourPanel.FocusFirstElement();
    }
}
