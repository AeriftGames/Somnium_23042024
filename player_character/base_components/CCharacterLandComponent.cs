using Godot;
using System;

public partial class CCharacterLandComponent : Node3D
{
    [Signal] public delegate void LandCompleteEventHandler();

    private FpsCharacterBase ourCharacter = null;

    private AnimationPlayer animPlayer = null;
    private AudioStreamPlayer AudioStreamPlayerLand;

    public all_material_surfaces AllMaterialSurfaces = null;


    public void PostInit(FpsCharacterBase newCharacterBase)
    {
        ourCharacter = newCharacterBase;

        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer_Land");
        AudioStreamPlayerLand = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        // nacteni vsech dat material surfaces
        AllMaterialSurfaces =
            (all_material_surfaces)GD.Load("res://player/material_surface/all_material_surfaces.tres");
    }

    public void DoLandEffect()
    {
        animPlayer.Play("Land");
    }

    public void _on_animation_player_land_animation_finished(StringName animName)
    {
        EmitSignal(nameof(LandComplete));
    }

    public void PlayLandSoundNow() { PlayLandSound(); }

    public async void PlayLandSound(float addOffsetVolume = 0.0f, float addOffsetPitch = 0.0f)
    {
        await ToSignal(GetTree(), "physics_frame");

        // Detect materal surface name and play specific audio set of footsteps
        all_material_surfaces.EMaterialSurface materialSurface =
            AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());

        if (materialSurface != all_material_surfaces.EMaterialSurface.None)
        {
            // Play random footsteps sound by material surface
            UniversalFunctions.PlayRandomSound(
                AudioStreamPlayerLand,
                AllMaterialSurfaces.GetAudioArray(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing),
                AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing) + addOffsetVolume,
                AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Landing) + addOffsetPitch);
        }
    }

    private string DetectSurfaceMaterialOfFloor()
    {
        PhysicsDirectSpaceState3D directSpace = GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = ourCharacter.GlobalPosition + (Vector3.Up * 0.2f);
        rayParam.To = ourCharacter.GlobalPosition + (Vector3.Down * 1);

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
        {
            Node HitCollider = (Node)rayResult["collider"];
            if (HitCollider == null) return "none";

            if (HitCollider.IsInGroup("material_surface_metal"))
                return "material_surface_metal";

            if (HitCollider.IsInGroup("material_surface_wood"))
                return "material_surface_wood";
        }

        return "none";
    }
}
