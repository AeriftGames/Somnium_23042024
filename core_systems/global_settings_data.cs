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
	[Export] public int GlobalIlumination { get; set; }
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
	[Export] public bool ShowDebugLabels {  get; set; }
	[Export] public bool ShowPerformance { get; set; }
	[Export] public bool EnableWorldLevelOcclusionCull { get; set; }

    [ExportGroupAttribute("Inputs Settings")]
    [Export] public float LookMouseSmooth { get; set; }
    [Export] public float LookMouseSensitivity { get; set; }
    [Export] public float LookGamepadSmooth { get; set; }
    [Export] public float LookGamepadSensitivity { get; set; }
	[Export] public bool InverseVerticalLook { get; set; }

    public global_settings_data()
	{
	}

	public void Save()
	{
		ResourceSaver.Save(this);
	}
}
