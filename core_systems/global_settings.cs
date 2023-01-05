using Godot;
using System;

public partial class global_settings : Godot.Object
{
	private GameMaster gm;

	public global_settings(Node ownerInstance)
	{
		gm = (GameMaster)ownerInstance;
	}

	// tady ziskavame pristup k hodnotam ulozenych v global_settings_data.tres
	public global_settings_data GetData()
	{
        Resource Settings = GD.Load("res://global_settings.tres");
        if (Settings != null && Settings is global_settings_data settings_data)
			return settings_data;
		else
			return null;          
    }

	// tady ulozime hodnoty na disk
	public void SaveData()
	{
		Resource Settings = GD.Load("res://global_settings.tres");
		if (Settings != null && Settings is global_settings_data settings_data)
			settings_data.Save();
	}

	/**************************************************************************/
	// Graphics

	public void Set_Ssao(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
	{
		// Apply now
		if(newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
		{
			Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
				("WorldEnvironment").Environment;

			env.SsaoEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "ssao: " + newValue);
        }

		// Save now
		if(newSaveNow)
		{
			GetData().Ssao = newValue;
			gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings data: ssao: " + newValue);
		}
	}

    public void Set_Ssil(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
        {
            Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

            env.SsilEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "ssil: " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Ssil = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings data: ssil: " + newValue);
        }
    }

    public void Set_Sdfgi(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
        {
            Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

            env.SdfgiEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "sdfgi: " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Sdfgi = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings data: sdfgi: " + newValue);
        }
    }

    public void Set_Antialias(int newID, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if(newApplyNow)
        {
            if (newID == 0)
            {
                //disable
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "antialias: disable");

            }
            else if (newID == 1)
            {
                //only ss_aa
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "antialias: only ss_aa");
            }
            else if (newID == 2)
            {
                //ss_aa+taa
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "antialias: ss_aa + taa");
            }
            else if (newID == 3)
            {
                //only msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Msaa2x;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "antialias: only msaa3d_2x");
            }
            else if (newID == 4)
            {
                //ss_aa+taa+msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Msaa2x;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "antialias: ss_aa + taa + msaa3d_2x");
            }
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Antialias = newID;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings data: antialias = " + newID);
        }
    }

    public void Set_Scale3D(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if(newApplyNow)
        {
            gm.GetTree().Root.Scaling3dScale = newValue;
        }

        // Save now
        if(newSaveNow)
        {
            GetData().Scale3d = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings data: scale 3d = " + newValue);
        }
    }

    public void Set_HalfResolutionGI(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            RenderingServer.GiSetUseHalfResolution(newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().HalfResolutionGI = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings data: half resolution gi = " + newValue);
        }
    }
}
