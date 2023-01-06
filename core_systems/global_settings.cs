using Godot;
using System;

public partial class global_settings : Godot.Object
{
	private GameMaster gm;

    // promene ktere slouzi pro otestovani aktualniho stavu a nejde to jinak
    private bool actual_halfResolutionGI = false;

	public global_settings(Node ownerInstance)
	{
		gm = (GameMaster)ownerInstance;
	}

	// tady ziskavame pristup k hodnotam ulozenych v global_settings_data.tres
	public global_settings_data GetData()
	{
        Resource Settings = GD.Load("res://global_settings_data.tres");
        if (Settings != null && Settings is global_settings_data settings_data)
			return settings_data;
		else
			return null;          
    }

	/**************************************************************************/
	// Graphics

    // Spoustime v player startu pri startu hry (aby se mohl nastavit WorldEnvironment)
    public void LoadAndApply_AllGraphicsSettings()
    {
        // nacteme veskera data ulozena ze souboru
        global_settings_data data = GetData();

        // pouze aplikujeme jednotliva nastaveni = neukladame do souboru
        Apply_Ssao(data.Ssao, true, false);
        Apply_Ssil(data.Ssil, true, false);
        Apply_Scale3D(data.Scale3d,true, false);
        Apply_HalfResolutionGI(data.HalfResolutionGI,true, false);
        Apply_AntialiasID(data.Antialias,true, false);
        Apply_Sdfgi(data.Sdfgi, true, false);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all graphics data is apply");
    }

    // Toto volani projede veskera aktualni aplikovana graficka nastaveni a
    // ulozi je do souboru global_settings_data.tres
    public void SaveActual_AllGraphicsSettings()
    {
        global_settings_data data = GetData();

        // neaplikujeme, pouze ulozime jednotliva aktualni nastaveni do souboru
        Apply_Ssao(GetActual_Ssao(), false, true);
        Apply_Ssil(GetActual_Ssil(), false, true);
        Apply_Sdfgi(GetActual_Sdfgi(), false, true);
        Apply_AntialiasID(GetActual_AntialiasID(), false, true);
        Apply_Scale3D(GetActual_Scale3D(), false, true);
        Apply_HalfResolutionGI(GetActual_HalfResolutionGI(), false, true);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all graphics data is saved");
    }

    // settings SSAO
	public void Apply_Ssao(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
	{
		// Apply now
		if(newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
		{
			Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
				("WorldEnvironment").Environment;

			env.SsaoEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: ssao = " + newValue);
        }

		// Save now
		if(newSaveNow)
		{
			GetData().Ssao = newValue;
            GetData().Save();
			gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings: ssao = " + newValue);
		}
	}

    public bool GetActual_Ssao()
    {
        Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

        return env.SsaoEnabled;
    }

    // settings SSIL
    public void Apply_Ssil(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
        {
            Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

            env.SsilEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: ssil = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Ssil = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings: ssil = " + newValue);
        }
    }

    public bool GetActual_Ssil()
    {
        Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

        return env.SsilEnabled;
    }

    // SDFGI
    public void Apply_Sdfgi(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
        {
            Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

            env.SdfgiEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: sdfgi = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Sdfgi = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings: sdfgi = " + newValue);
        }
    }

    public bool GetActual_Sdfgi()
    {
        Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

        return env.SdfgiEnabled;
    }

    // ANTIALIAS ID
    public void Apply_AntialiasID(int newAntialiasID, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (newAntialiasID == 0)
            {
                //disable
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: antialias = disable");

            }
            else if (newAntialiasID == 1)
            {
                //only ss_aa
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: antialias = only ss_aa");
            }
            else if (newAntialiasID == 2)
            {
                //ss_aa+taa
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: antialias = ss_aa + taa");
            }
            else if (newAntialiasID == 3)
            {
                //only msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Msaa2x;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: antialias = only msaa3d_2x");
            }
            else if (newAntialiasID == 4)
            {
                //ss_aa+taa+msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Msaa2x;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: antialias = ss_aa + taa + msaa3d_2x");
            }
            else
            {
                //chyba - mimo rozsah id
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "pokus o chybne nastaveni antialias id," +
                    " hodnotou: " + newAntialiasID);
            }
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Antialias = newAntialiasID;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings: antialias = " + newAntialiasID);
        }
    }

    public int GetActual_AntialiasID()
    {
        if (gm.GetTree().Root.ScreenSpaceAa == Viewport.ScreenSpaceAA.Disabled &&
            gm.GetTree().Root.UseTaa == false &&
            gm.GetTree().Root.Msaa3d == Viewport.MSAA.Disabled)
        {
            //disable
            return 0;
        }
        else if (gm.GetTree().Root.ScreenSpaceAa == Viewport.ScreenSpaceAA.Fxaa &&
            gm.GetTree().Root.UseTaa == false &&
            gm.GetTree().Root.Msaa3d == Viewport.MSAA.Disabled)
        {
            //ss_aa
            return 1;
        }
        else if (gm.GetTree().Root.ScreenSpaceAa == Viewport.ScreenSpaceAA.Fxaa &&
            gm.GetTree().Root.UseTaa == true &&
            gm.GetTree().Root.Msaa3d == Viewport.MSAA.Disabled)
        {
            //ss_aa+taa
            return 2;
        }
        else if (gm.GetTree().Root.ScreenSpaceAa == Viewport.ScreenSpaceAA.Disabled &&
            gm.GetTree().Root.UseTaa == false &&
            gm.GetTree().Root.Msaa3d == Viewport.MSAA.Msaa2x)
        {
            //only msaa3d_2x
            return 3;
        }
        else if (gm.GetTree().Root.ScreenSpaceAa == Viewport.ScreenSpaceAA.Fxaa &&
            gm.GetTree().Root.UseTaa == true &&
            gm.GetTree().Root.Msaa3d == Viewport.MSAA.Msaa2x)
        {
            //ss_aa+taa+msaa3d_2x
            return 4;
        }
        else
        {
            //error - mimo rozsah id
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "chybne vraceni hodnoty z GetActual_AntialiasID," +
                " nejspis je jiny nastaveni antialias nez dane id presety");
            return -1;
        }
    }

    // SCALE 3D
    public void Apply_Scale3D(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if(newApplyNow)
        {
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: scale 3d = " + newValue);
            gm.GetTree().Root.Scaling3dScale = newValue;
        }

        // Save now
        if(newSaveNow)
        {
            GetData().Scale3d = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings: scale 3d = " + newValue);
        }
    }

    public float GetActual_Scale3D()
    {
        return gm.GetTree().Root.Scaling3dScale;
    }

    // HALF RESOLUTION GI
    public void Apply_HalfResolutionGI(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            RenderingServer.GiSetUseHalfResolution(newValue);
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply settings: half resolutoin gi = " + newValue);

            // musime ulozit novy stav bokem, jinak by jsme pozdeji nemohli ziskat aktualni stav
            actual_halfResolutionGI = newValue;
        }

        // Save now
        if (newSaveNow)
        {
            GetData().HalfResolutionGI = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save settings: half resolution gi = " + newValue);
        }
    }

    public bool GetActual_HalfResolutionGI()
    {
        return actual_halfResolutionGI;
    }
}
