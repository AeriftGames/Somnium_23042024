using Godot;
using System;

public partial class CPanelMain : PanelContainer
{
	private CDebugPanel ourDebugPanel = null;

	public void PostInit(CDebugPanel newDebugPanel){ ourDebugPanel = newDebugPanel; }

	public void _on_button_quit_game_pressed(){ CGameMaster.GM.QuitGame(); }
	public void _on_check_button_show_fps_toggled( bool newToggleOn ){ ourDebugPanel.SetShowFps(newToggleOn); }
	public void _on_check_button_show_debug_labels_toggled(bool newToggleOn) 
	{
		ourDebugPanel.GetDebugLabels().SetActive(newToggleOn);
	}
}
