using Godot;
using Godot.Collections;
using System;

public partial class material_surface_data : Resource
{
    [Export] public string MaterialSurfaceNameID { get; set; }
    [Export] public all_material_surfaces.EMaterialSurface MaterialSurface { get; set; }

    [ExportGroupAttribute("Character Footstep")]
    [Export] public Array<AudioStream> FootstepSounds { get; set; }
    [Export] public float FootstepAudioPitchScale { get; set;}
    [Export] public float FootstepAudioVolumeDB { get; set;}

    [ExportGroupAttribute("Character Landing")]
    [Export] public Array<AudioStream> LandingSounds { get; set; }
    [Export] public float LandingAudioPitchScale { get; set; }
    [Export] public float LandingAudioVolumeDB { get; set; }

    [ExportGroupAttribute("Hit Object")]
    [Export] public Array<AudioStream> HitObjectSounds { get; set; }
    [Export] public float HitObjectAudioPitchScale { get; set; }
    [Export] public float HitObjectAudioVolumeDB { get; set; }

    [ExportGroupAttribute("Drag Object")]
    [Export] public Array<AudioStream> DragObjectSounds { get; set; }
    [Export] public float DragObjectAudioPitchScale { get; set; }
    [Export] public float DragObjectAudioVolumeDB { get; set; }
}
