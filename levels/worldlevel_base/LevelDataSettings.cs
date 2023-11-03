using Godot;
using System;

public partial class LevelDataSettings : Node
{
    public enum EQualityPresset { lowest=0,low=1,medium=2,high=3,highest=4}

    [Export] public bool ForceQualityOnLevelStart = false;

    [Export] public EQualityPresset StartQualityPresset = EQualityPresset.high;

    [ExportGroup("Lowest Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum LowestVoxelSub = VoxelGI.SubdivEnum.Subdiv128;
    [Export] public float LowestVoxelHdr = 2.0f;
    [Export] public float LowestVoxelEnergy = 0.6f;
    [Export] public float LowestVoxelBias = 0.3f;
    [Export] public float LowestVoxelNormalBias = 0.6f;
    [Export] public float LowestVoxelPropag = 0.7f;

    [ExportGroup("Low Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum LowVoxelSub = VoxelGI.SubdivEnum.Subdiv128;
    [Export] public float LowVoxelHdr = 2.0f;
    [Export] public float LowVoxelEnergy = 0.6f;
    [Export] public float LowVoxelBias = 0.3f;
    [Export] public float LowVoxelNormalBias = 0.6f;
    [Export] public float LowVoxelPropag = 0.7f;

    [ExportGroup("Medium Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum MediumVoxelSub = VoxelGI.SubdivEnum.Subdiv256;
    [Export] public float MediumVoxelHdr = 2.0f;
    [Export] public float MediumVoxelEnergy = 0.7f;
    [Export] public float MediumVoxelBias = 0.6f;
    [Export] public float MediumVoxelNormalBias = 0.6f;
    [Export] public float MediumVoxelPropag = 0.8f;

    [ExportGroup("High Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum HighVoxelSub = VoxelGI.SubdivEnum.Subdiv512;
    [Export] public float HighVoxelHdr = 2.0f;
    [Export] public float HighVoxelEnergy = 0.8f;
    [Export] public float HighVoxelBias = 0.8f;
    [Export] public float HighVoxelNormalBias = 1.2f;
    [Export] public float HighVoxelPropag = 0.8f;

    [ExportGroup("Highest Voxel Settings")]
    [Export] public VoxelGI.SubdivEnum HighestVoxelSub = VoxelGI.SubdivEnum.Subdiv512;
    [Export] public float HighestVoxelHdr = 2.0f;
    [Export] public float HighestVoxelEnergy = 0.8f;
    [Export] public float HighestVoxelBias = 0.8f;
    [Export] public float HighestVoxelNormalBias = 1.2f;
    [Export] public float HighestVoxelPropag = 0.8f;


    public class SQualityData
    {
        public float FSRScale;              //0.2 - 1.0
        public bool SSAO;                   // false - true
        public bool SSIL;                   // false - true
        public VoxelGI.SubdivEnum VoxelSub; // 64,128,256,512
        public float VoxelHdr;
        public float VoxelEnergy;             // 0-4
        public float VoxelBias;
        public float VoxelNormalBias;
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
        newPresset.VoxelHdr = LowestVoxelHdr;
        newPresset.VoxelEnergy = LowestVoxelEnergy;
        newPresset.VoxelBias = LowestVoxelBias;
        newPresset.VoxelNormalBias = LowestVoxelNormalBias;
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
        newPresset.VoxelHdr = LowVoxelHdr;
        newPresset.VoxelEnergy = LowVoxelEnergy;
        newPresset.VoxelBias = LowVoxelBias;
        newPresset.VoxelNormalBias = LowVoxelNormalBias;
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
        newPresset.VoxelHdr = MediumVoxelHdr;
        newPresset.VoxelEnergy = MediumVoxelEnergy;
        newPresset.VoxelBias = MediumVoxelBias;
        newPresset.VoxelNormalBias = MediumVoxelNormalBias;
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
        newPresset.VoxelHdr = HighVoxelHdr;
        newPresset.VoxelEnergy = HighVoxelEnergy;
        newPresset.VoxelBias = HighVoxelBias;
        newPresset.VoxelNormalBias = HighVoxelNormalBias;
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

        newPresset.VoxelSub = HighestVoxelSub;
        newPresset.VoxelHdr = HighestVoxelHdr;
        newPresset.VoxelEnergy = HighestVoxelEnergy;
        newPresset.VoxelBias = HighestVoxelBias;
        newPresset.VoxelNormalBias = HighestVoxelNormalBias;
        newPresset.VoxelPropag = HighestVoxelPropag;

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
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.Bias = newLevelData.VoxelBias;
        GameMaster.GM.LevelLoader.GetActualLevelScene().GetVoxelGI().Data.NormalBias = newLevelData.VoxelNormalBias;

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
