using Godot;
using System;

public partial class DebugPanelTabBar : TabBar
{
    public void _on_focus_entered()
    {
        // we try get node about two levels in and try get as CPanelBase
        // if success, we focus first element in panel
        MarginContainer ourMargin = GetChild<MarginContainer>(0);
        if (ourMargin != null ) 
        {
            CPanelBase ourPanel = ourMargin.GetChild<CPanelBase>(0);
            if (ourPanel != null)
                ourPanel.FocusFirstElement();
        }
    }
}
