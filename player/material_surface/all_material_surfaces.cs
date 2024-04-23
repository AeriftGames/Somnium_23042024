using Godot;
using Godot.Collections;
using System;

public partial class all_material_surfaces : Resource
{
    public enum EMaterialSurface { None, Metal, Wood }
    public enum EMaterialSurfaceAudio { Footstep, Landing, HitObject, DragObject }

    [Export] private Array<material_surface_data> AllMaterialSurfaces;

    public EMaterialSurface GetMaterialSurfaceFromGroup(string newMaterialSurfaceGroup)
    {
        switch (newMaterialSurfaceGroup)
        {
            case "material_surface_metal":
                return EMaterialSurface.Metal;
            case "material_surface_wood":
                return EMaterialSurface.Wood;
            default:
                return EMaterialSurface.None;
        }
    }

    public Array<material_surface_data> GetAllMaterialSurfaces(){ return AllMaterialSurfaces;}

    public  Array<AudioStream>GetAudioArray(EMaterialSurface newSurface,EMaterialSurfaceAudio newAudio)
    {
        if(AllMaterialSurfaces == null) { return null;}
        if(AllMaterialSurfaces.Count > 0)
        {
            foreach(material_surface_data surface_data in AllMaterialSurfaces) 
            {
                if(newSurface == surface_data.MaterialSurface)
                {
                    switch (newAudio)
                    {
                        case EMaterialSurfaceAudio.Footstep:
                            return surface_data.FootstepSounds;
                        case EMaterialSurfaceAudio.Landing:
                            return surface_data.LandingSounds;
                        case EMaterialSurfaceAudio.HitObject:
                            return surface_data.HitObjectSounds;
                        case EMaterialSurfaceAudio.DragObject:
                            return surface_data.DragObjectSounds;
                        default:
                            break;
                    }
                }
            }
        }

        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.WARNING,
            "(AllMaterialSurfaces) audio - no surface find");
        return null;
    }

    public float GetMaterialSurfaceAudioPitch(EMaterialSurface newSurface, EMaterialSurfaceAudio newAudio)
    {
        if (AllMaterialSurfaces == null) { return 0.0f; }
        if (AllMaterialSurfaces.Count > 0)
        {
            foreach (material_surface_data surface_data in AllMaterialSurfaces)
            {
                if (newSurface == surface_data.MaterialSurface)
                {
                    switch (newAudio)
                    {
                        case EMaterialSurfaceAudio.Footstep:
                            return surface_data.FootstepAudioPitchScale;
                        case EMaterialSurfaceAudio.Landing:
                            return surface_data.LandingAudioPitchScale;
                        case EMaterialSurfaceAudio.HitObject:
                            return surface_data.HitObjectAudioPitchScale;
                        case EMaterialSurfaceAudio.DragObject:
                            return surface_data.DragObjectAudioPitchScale;
                        default:
                            break;
                    }
                }
            }
        }

        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.WARNING,
            "(AllMaterialSurfaces) pitch - no surface find");
        return 0.0f;
    }

    public float GetMaterialSurfaceAudioVolumeDB(EMaterialSurface newSurface, EMaterialSurfaceAudio newAudio)
    {
        if (AllMaterialSurfaces == null) { return 0.0f; }
        if (AllMaterialSurfaces.Count > 0)
        {
            foreach (material_surface_data surface_data in AllMaterialSurfaces)
            {
                if (newSurface == surface_data.MaterialSurface)
                {
                    switch (newAudio)
                    {
                        case EMaterialSurfaceAudio.Footstep:
                            return surface_data.FootstepAudioVolumeDB;
                        case EMaterialSurfaceAudio.Landing:
                            return surface_data.LandingAudioVolumeDB;
                        case EMaterialSurfaceAudio.HitObject:
                            return surface_data.HitObjectAudioVolumeDB;
                        case EMaterialSurfaceAudio.DragObject:
                            return surface_data.DragObjectAudioVolumeDB;
                        default:
                            break;
                    }
                }
            }
        }

        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM,CMasterLog.ELogMsgType.WARNING,
            "(AllMaterialSurfaces) volumedb - no surface find");
        return 0.0f;
    }
}
