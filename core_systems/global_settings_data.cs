using Godot;
using System;
using System.Net.Security;
using System.Xml.Schema;

public partial class global_settings_data : Resource
{
	[ExportGroupAttribute("Graphics Settings")]
	[Export] public int ScreenMode { get; set; }
	[Export] public int ScreenSizeID { get; set; }
	[Export] public float Scale3d { get; set; }
	[Export] public int AntialiasID { get; set; }
	[Export] public bool Sdfgi { get; set; }
	[Export] public bool Ssao { get; set; }
	[Export] public bool Ssil { get; set; }
	[Export] public bool HalfResolutionGI { get; set; }
	[Export] public bool UnlockMaxFps { get; set; }
	[Export] public bool DisableVsync { get; set; }

	[ExportGroupAttribute("Audio Settings")]
	[Export] public float MainVolume { get; set; }
	[Export] public float MusicVolume { get; set; }
	[Export] public float SfxVolume { get; set; }

	[ExportGroupAttribute("Debug Hud Settings")]
	[Export] public bool ShowDebugHud { get; set; }
	[Export] public bool ShowFps { get; set; }
	[Export] public bool ShowPerformance { get; set; }
	[Export] public bool ShowCustomLabel0 { get; set; }
	[Export] public bool ShowCustomLabel1 { get; set; }
	[Export] public bool ShowCustomLabel2 { get; set; }
	[Export] public bool ShowCustomLabel3 { get; set; }
	[Export] public bool ShowCustomLabel4 { get; set; }
	[Export] public bool ShowCustomLabel5 { get; set; }
	[Export] public bool ShowCustomLabel6 { get; set; }
	[Export] public bool ShowCustomLabel7 { get; set; }
	[Export] public bool ShowCustomLabel8 { get; set; }
	[Export] public bool ShowCustomLabel9 { get; set; }
	[Export] public bool EnableWorldLevelOcclusionCull { get; set; }


	public global_settings_data()
	{
	}

	public void Save()
	{
		ResourceSaver.Save(this);
	}
}
