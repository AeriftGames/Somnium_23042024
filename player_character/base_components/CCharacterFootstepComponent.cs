using Godot;
using System;

public partial class CCharacterFootstepComponent : Node3D
{
    [Export] public float FootstepsVolumeDBInWalk = -2.0f;
    [Export] public float FootstepsVolumeDBInSprint = 1.0f;
    [Export] public float FootstepsVolumeDBInCrouch = -7.0f;
    [Export] public float FootstepsAudioPitch = 0.75f;

    private FpsCharacterBase ourCharacterBase = null;

    private AudioStreamPlayer AudioStreamPlayerFootsteps;

    public all_material_surfaces AllMaterialSurfaces = null;

    public void PostInit(FpsCharacterBase newCharacterBase)
    {
        ourCharacterBase = newCharacterBase;

        AudioStreamPlayerFootsteps = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Footsteps");

        // nacteni vsech dat material surfaces
        AllMaterialSurfaces =
            (all_material_surfaces)GD.Load("res://player/material_surface/all_material_surfaces.tres");
    }

    public async void PlayFootstepNow()
    {

        GD.Print("Footstep");

        await ToSignal(GetTree(), "physics_frame");

        if (ourCharacterBase.GetCharacterStateMachine().GetCurrentStateName() == "WalkingPlayerState")
            PlayFootstepSound(FootstepsVolumeDBInWalk, FootstepsAudioPitch);
    }

    public void PlayFootstepSound(float addOffsetVolume = 0.0f, float addOffsetPitch = 0.0f)
    {
        // Detect materal surface name and play specific audio set of footsteps
        all_material_surfaces.EMaterialSurface materialSurface =
            AllMaterialSurfaces.GetMaterialSurfaceFromGroup(DetectSurfaceMaterialOfFloor());

        if (materialSurface != all_material_surfaces.EMaterialSurface.None)
        {
            // Play random footsteps sound by material surface
            UniversalFunctions.PlayRandomSound(
                AudioStreamPlayerFootsteps,
                AllMaterialSurfaces.GetAudioArray(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep),
                AllMaterialSurfaces.GetMaterialSurfaceAudioVolumeDB(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep) + addOffsetVolume,
                AllMaterialSurfaces.GetMaterialSurfaceAudioPitch(
                    materialSurface, all_material_surfaces.EMaterialSurfaceAudio.Footstep) + addOffsetPitch);
        }
    }

    private string DetectSurfaceMaterialOfFloor()
    {
        PhysicsDirectSpaceState3D directSpace = GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = ourCharacterBase.GlobalPosition + (Vector3.Up * 0.2f);
        rayParam.To = ourCharacterBase.GlobalPosition + (Vector3.Down * 1);

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
