using Godot;
using System;

public partial class LevelDataSettings : Node
{
    public enum EQualityPresset { lowest=0,low=1,medium=2,high=3,highest=4}

    [Export] public bool ForceQualityOnLevelStart = false;

    [Export] public EQualityPresset StartQualityPresset = EQualityPresset.medium;

    [ExportGroup("Lowest Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum LowestVoxelSub = VoxelGI.SubdivEnum.Subdiv128;
    [Export] public float LowestVoxelEnergy = 0.5f;
    [Export] public float LowestVoxelPropag = 0.5f;

    [ExportGroup("Low Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum LowVoxelSub = VoxelGI.SubdivEnum.Subdiv128;
    [Export] public float LowVoxelEnergy = 0.7f;
    [Export] public float LowVoxelPropag = 0.7f;

    [ExportGroup("Medium Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum MediumVoxelSub = VoxelGI.SubdivEnum.Subdiv256;
    [Export] public float MediumVoxelEnergy = 0.8f;
    [Export] public float MediumVoxelPropag = 0.8f;

    [ExportGroup("High Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum HighVoxelSub = VoxelGI.SubdivEnum.Subdiv512;
    [Export] public float HighVoxelEnergy = 0.8f;
    [Export] public float HighVoxelPropag = 0.82f;

    [ExportGroup("Highest Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum HighestVoxelSub = VoxelGI.SubdivEnum.Subdiv512;
    [Export] public float HighestVoxelEnergy = 0.8f;
    [Export] public float HighestVoxelPropag = 0.82f;


    public class SQualityData
    {
        public float FSRScale;              //0.2 - 1.0
        public bool SSAO;                   // false - true
        public bool SSIL;                   // false - true
        public VoxelGI.SubdivEnum VoxelSub; // 64,128,256,512
        public float VoxelEnergy;             // 0-4
        public float VoxelPropag;             // 0-1
        public RenderingServer.ShadowQuality ShadowFilterQuality;
        public int ShadowAtlasSize;         // 1024-8192

    }

    public SQualityData GetLowestPresset()
    {
        SQualityData newPresset = new SQualityData();

        newPresset.FSRScale = 0.33f;
        newPresset.SSAO = true;
        newPresset.SSIL = false;
        newPresset.ShadowFilterQuality = RenderingServer.ShadowQuality.SoftLow;
        newPresset.ShadowAtlasSize = 2048;

        newPresset.VoxelSub = LowestVoxelSub;
        newPresset.VoxelEnergy = LowestVoxelEnergy;
        newPresset.VoxelPropag = LowestVoxelPropag;

        return newPresset;
    }

    public SQualityData GetLowPresset()
    {
        SQualityData newPresset = new SQualityData();

        newPresset.FSRScale = 0.36f;
        newPresset.SSAO = true;
        newPresset.SSIL = true;
        newPresset.ShadowFilterQuality = RenderingServer.ShadowQuality.SoftLow;
        newPresset.ShadowAtlasSize = 2048;

        newPresset.VoxelSub = LowVoxelSub;
        newPresset.VoxelEnergy = LowVoxelEnergy;
        newPresset.VoxelPropag = LowVoxelPropag;

        return newPresset;
    }

    public SQualityData GetMediumPresset()
    {
        SQualityData newPresset = new SQualityData();

        newPresset.FSRScale = 0.5f;
        newPresset.SSAO = true;
        newPresset.SSIL = true;
        newPresset.ShadowFilterQuality = RenderingServer.ShadowQuality.SoftMedium;
        newPresset.ShadowAtlasSize = 4096;

        newPresset.VoxelSub = MediumVoxelSub;
        newPresset.VoxelEnergy = MediumVoxelEnergy;
        newPresset.VoxelPropag = MediumVoxelPropag;

        return newPresset;
    }

    public SQualityData GetHighPresset()
    {
        SQualityData newPresset = new SQualityData();

        newPresset.FSRScale = 0.8f;
        newPresset.SSAO = true;
        newPresset.SSIL = true;
        newPresset.ShadowFilterQuality = RenderingServer.ShadowQuality.SoftHigh;
        newPresset.ShadowAtlasSize = 4096;

        newPresset.VoxelSub = HighVoxelSub;
        newPresset.VoxelEnergy = HighVoxelEnergy;
        newPresset.VoxelPropag = HighVoxelPropag;

        return newPresset;
    }

    public SQualityData GetHighestPresset()
    {
        SQualityData newPresset = new SQualityData();

        newPresset.FSRScale = 0.98f;
        newPresset.SSAO = true;
        newPresset.SSIL = true;
        newPresset.VoxelSub = HighestVoxelSub;
        newPresset.VoxelEnergy = HighestVoxelEnergy;
        newPresset.VoxelPropag = HighestVoxelPropag;
        newPresset.ShadowFilterQuality = RenderingServer.ShadowQuality.SoftUltra;
        newPresset.ShadowAtlasSize = 8192;

        return newPresset;
    }

    public async void ApplyNewLevelDataSettings(SQualityData newLevelData,bool newSave = false)
    {

        GameMaster.GM.GetSettings().Apply_Scale3D(newLevelData.FSRScale, true, newSave);
        GameMaster.GM.GetSettings().Apply_Ssao(newLevelData.SSAO, true, newSave);
        GameMaster.GM.GetSettings().Apply_Ssil(newLevelData.SSIL,true, newSave);

        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Subdiv = newLevelData.VoxelSub;

        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Energy = newLevelData.VoxelEnergy;
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Propagation = newLevelData.VoxelPropag;

        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().CallDeferred("bake");
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.EmitChanged();

        GD.Print(GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.HasSignal("changed"));
        await ToSignal(GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data, "changed");

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Energy = newLevelData.VoxelEnergy;
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Propagation = newLevelData.VoxelPropag;
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.EmitChanged();

        ProjectSettings.SetSetting("rendering/lights_and_shadows/positional_shadow/soft_shadow_filter_quality",
            (int)newLevelData.ShadowFilterQuality);

        ProjectSettings.SetSetting("rendering/lights_and_shadows/positional_shadow/atlas_size", newLevelData.ShadowAtlasSize);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("quality1"))
            ApplyNewLevelDataSettings(GetLowestPresset());
        else if (Input.IsActionJustPressed("quality2"))
            ApplyNewLevelDataSettings(GetLowPresset());
        else if (Input.IsActionJustPressed("quality3"))
            ApplyNewLevelDataSettings(GetMediumPresset());
        else if (Input.IsActionJustPressed("quality4"))
            ApplyNewLevelDataSettings(GetHighPresset());
        else if (Input.IsActionJustPressed("quality5"))
            ApplyNewLevelDataSettings(GetHighestPresset());
        else if (Input.IsActionJustPressed("change_energy"))
        {
            GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Energy = 0.15f;
            GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.EmitChanged();
        }
        else if (Input.IsActionJustPressed("change_energy_plus"))
        {
            GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Energy = 1.8f;
            GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.EmitChanged();
        }
    }

    public override void _Ready()
    {
        base._Ready();

        if(ForceQualityOnLevelStart)
            SetQualityPresset();
    }

    public async void SetQualityPresset()
    {
        await ToSignal(GetTree().CreateTimer(1), "timeout");

        switch (StartQualityPresset)
        {
            case EQualityPresset.lowest:
                ApplyNewLevelDataSettings(GetLowestPresset());
                break;

            case EQualityPresset.low:
                ApplyNewLevelDataSettings(GetLowPresset());
                break;

            case EQualityPresset.medium:
                ApplyNewLevelDataSettings(GetMediumPresset());
                break;

            case EQualityPresset.high:
                ApplyNewLevelDataSettings(GetHighPresset());
                break;

            case EQualityPresset.highest:
                ApplyNewLevelDataSettings(GetHighestPresset());
                break;
        }
    }
}
