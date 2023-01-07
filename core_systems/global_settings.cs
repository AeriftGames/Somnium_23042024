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

        // Audio volumes init
        LoadAndApply_AllAudioSettings();
        SaveActual_AllAudioSettings();
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
	// GRAPHICS

    // Spoustime v player startu pri startu hry (aby se mohl nastavit WorldEnvironment)
    public void LoadAndApply_AllGraphicsSettings()
    {
        // nacteme veskera data ulozena ze souboru
        global_settings_data data = GetData();

        // pouze aplikujeme jednotliva nastaveni = neukladame do souboru
        Apply_ScreenMode(data.ScreenMode, true, false);
        Apply_WindowSizeID(data.WindowSizeID, true, false);
        Apply_Ssao(data.Ssao, true, false);
        Apply_Ssil(data.Ssil, true, false);
        Apply_Scale3D(data.Scale3d,true, false);
        Apply_HalfResolutionGI(data.HalfResolutionGI,true, false);
        Apply_AntialiasID(data.AntialiasID,true, false);
        Apply_Sdfgi(data.Sdfgi, true, false);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all graphics data is apply");
    }

    // Toto volani projede veskera aktualni aplikovana graficka nastaveni a
    // ulozi je do souboru global_settings_data.tres
    public void SaveActual_AllGraphicsSettings()
    {
        global_settings_data data = GetData();

        // neaplikujeme, pouze ulozime jednotliva aktualni nastaveni do souboru
        Apply_ScreenMode(GetActual_ScreenMode(), false, true);
        Apply_WindowSizeID(GetActual_WindowSizeID(), false, true);
        Apply_Ssao(GetActual_Ssao(), false, true);
        Apply_Ssil(GetActual_Ssil(), false, true);
        Apply_Sdfgi(GetActual_Sdfgi(), false, true);
        Apply_AntialiasID(GetActual_AntialiasID(), false, true);
        Apply_Scale3D(GetActual_Scale3D(), false, true);
        Apply_HalfResolutionGI(GetActual_HalfResolutionGI(), false, true);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all graphics data is saved");
    }

    // settings SCREEN MODE ID
    public void Apply_ScreenMode(int newScreenModeID, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (newScreenModeID == 0)
            {
                //Windowed
                gm.GetTree().Root.Mode = Window.ModeEnum.Windowed;
                Apply_WindowSizeID(GetData().WindowSizeID,true,false);
                gm.GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.KeepWidth;

                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings:" +
                    " screen mode id = windowed");

            }
            else if (newScreenModeID == 1)
            {
                //Fullscreen normal
                gm.GetTree().Root.Mode = Window.ModeEnum.ExclusiveFullscreen;
                gm.GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.KeepWidth;

                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings:" +
                    " screen mode id = fullscreen normal");
            }
            else if (newScreenModeID == 2)
            {
                //Fullscreen wide
                gm.GetTree().Root.Mode = Window.ModeEnum.ExclusiveFullscreen;
                gm.GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;

                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings:" +
                    " screen mode id = fullscreen wide");
            }
            else
            {
                //chyba - mimo rozsah id
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "pokus o chybne nastaveni screen mode id," +
                    " hodnotou: " + newScreenModeID);
            }
        }

        // Save now
        if (newSaveNow)
        {
            GetData().ScreenMode = newScreenModeID;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: screen mode id = " + newScreenModeID);
        }
    }

    public int GetActual_ScreenMode()
    {
        if (gm.GetTree().Root.Mode == Window.ModeEnum.Windowed)
        {
            //windowed
            return 0;
        }
        else if (gm.GetTree().Root.Mode == Window.ModeEnum.ExclusiveFullscreen && 
            gm.GetTree().Root.ContentScaleAspect == Window.ContentScaleAspectEnum.KeepWidth)
        {
            //fullscreen normal
            return 1;
        }
        else if (gm.GetTree().Root.Mode == Window.ModeEnum.ExclusiveFullscreen &&
            gm.GetTree().Root.ContentScaleAspect == Window.ContentScaleAspectEnum.Expand)
        {
            //fullscreen expand
            return 2;
        }
        else
        {
            //error - mimo rozsah id
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "chybne vraceni hodnoty z GetActual_ScreeMode," +
                " nejspis je jiny nastaveni screen mode nez dane id presety");
            return -1;
        }
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
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: ssao = " + newValue);
        }

		// Save now
		if(newSaveNow)
		{
			GetData().Ssao = newValue;
            GetData().Save();
			gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: ssao = " + newValue);
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
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: ssil = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Ssil = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: ssil = " + newValue);
        }
    }

    public bool GetActual_Ssil()
    {
        Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

        return env.SsilEnabled;
    }

    // settings SDFGI
    public void Apply_Sdfgi(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow && gm.LevelLoader.GetActualLevelScene() != null)
        {
            Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

            env.SdfgiEnabled = newValue;
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: sdfgi = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().Sdfgi = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: sdfgi = " + newValue);
        }
    }

    public bool GetActual_Sdfgi()
    {
        Godot.Environment env = gm.LevelLoader.GetActualLevelScene().GetNode<WorldEnvironment>
                ("WorldEnvironment").Environment;

        return env.SdfgiEnabled;
    }

    // settings ANTIALIAS ID
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
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = disable");

            }
            else if (newAntialiasID == 1)
            {
                //only ss_aa
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = only ss_aa");
            }
            else if (newAntialiasID == 2)
            {
                //ss_aa+taa
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = ss_aa + taa");
            }
            else if (newAntialiasID == 3)
            {
                //only msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Msaa2x;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = only msaa3d_2x");
            }
            else if (newAntialiasID == 4)
            {
                //ss_aa+taa+msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAa = Viewport.ScreenSpaceAA.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3d = Viewport.MSAA.Msaa2x;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = ss_aa + taa + msaa3d_2x");
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
            GetData().AntialiasID = newAntialiasID;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: antialias = " + newAntialiasID);
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

    // settings WINDOW SIZE ID
    public void Apply_WindowSizeID(int newWindowSizeID, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (newWindowSizeID == 0)
            {
                //1280x720
                gm.GetTree().Root.Size = new Vector2i(1280, 720);
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: window size id = 1280x720");

            }
            else if (newWindowSizeID == 1)
            {
                //1920x1080
                gm.GetTree().Root.Size = new Vector2i(1920, 1080);
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: window size id = 1920x1080");
            }
            else if(newWindowSizeID == 2)
            {
                //native
            }
            else
            {
                //chyba - mimo rozsah id
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "pokus o chybne nastaveni window size id," +
                    " hodnotou: " + newWindowSizeID);
            }
        }

        // Save now
        if (newSaveNow)
        {
            GetData().WindowSizeID = newWindowSizeID;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: window size id = " + newWindowSizeID);
        }
    }

    public int GetActual_WindowSizeID()
    {
        if (gm.GetTree().Root.Size.x == 1280 && gm.GetTree().Root.Size.y == 720)
        {
            //1280x720
            return 0;
        }
        else if (gm.GetTree().Root.Size.x == 1920 && gm.GetTree().Root.Size.y == 1080)
        {
            //1920x1080
            return 1;
        }
        else if(gm.GetTree().Root.Mode == Window.ModeEnum.ExclusiveFullscreen)
        {
            return 2;
        }
        else
        {
            //error - mimo rozsah id
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "chybne vraceni hodnoty z GetActual_WindowSizeID," +
                " nejspis je jiny nastaveni window size nez dane id presety");
            return -1;
        }
    }

    // settings SCALE 3D
    public void Apply_Scale3D(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if(newApplyNow)
        {
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: scale 3d = " + newValue);
            gm.GetTree().Root.Scaling3dScale = newValue;
        }

        // Save now
        if(newSaveNow)
        {
            GetData().Scale3d = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: scale 3d = " + newValue);
        }
    }

    public float GetActual_Scale3D()
    {
        return gm.GetTree().Root.Scaling3dScale;
    }

    // settings HALF RESOLUTION GI
    public void Apply_HalfResolutionGI(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            RenderingServer.GiSetUseHalfResolution(newValue);
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: half resolutoin gi = " + newValue);

            // musime ulozit novy stav bokem, jinak by jsme pozdeji nemohli ziskat aktualni stav
            actual_halfResolutionGI = newValue;
        }

        // Save now
        if (newSaveNow)
        {
            GetData().HalfResolutionGI = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: half resolution gi = " + newValue);
        }
    }

    public bool GetActual_HalfResolutionGI()
    {
        return actual_halfResolutionGI;
    }

    /**************************************************************************/
    // AUDIO

    // Spoustime
    public void LoadAndApply_AllAudioSettings()
    {
        // nacteme veskera data ulozena ze souboru
        global_settings_data data = GetData();

        // pouze aplikujeme jednotliva nastaveni = neukladame do souboru
        Apply_MainVolume(data.MainVolume, true, false);
        Apply_SfxVolume(data.SfxVolume, true, false);
        Apply_MusicVolume(data.MusicVolume, true, false);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all audio data is apply");
    }

    // Toto volani projede veskera aktualni aplikovana audio nastaveni a
    // ulozi je do souboru global_settings_data.tres
    public void SaveActual_AllAudioSettings()
    {
        global_settings_data data = GetData();

        // neaplikujeme, pouze ulozime jednotliva aktualni nastaveni do souboru
        Apply_MainVolume(GetActual_MainVolume(), false, true);
        Apply_SfxVolume(GetActual_SfxVolume(), false, true);
        Apply_MusicVolume(GetActual_MusicVolume(), false, true);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all audio data is saved");
    }

    // settings MAIN VOLUME
    public void Apply_MainVolume(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"),newValue);
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply audio settings: main volume = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().MainVolume = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save audio settings: main volume = " + newValue);
        }
    }

    public float GetActual_MainVolume()
    {
        return AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"));
    }

    // settings SFX VOLUME
    public void Apply_SfxVolume(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Sfx"), newValue);
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply audio settings: sfx volume = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().SfxVolume = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save audio settings: sfx volume = " + newValue);
        }
    }

    public float GetActual_SfxVolume()
    {
        return AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Sfx"));
    }

    // settings Music VOLUME
    public void Apply_MusicVolume(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), newValue);
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply audio settings: music volume = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().MusicVolume = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save audio settings: music volume = " + newValue);
        }
    }

    public float GetActual_MusicVolume()
    {
        return AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music"));
    }

    /**************************************************************************/
    // DEBUG HUD

    // settings SHOW DEBUG HUD
    public void Save_ShowDebugHud(bool newValue)
    {
        // Save now
        GetData().ShowDebugHud = newValue;
        GetData().Save();
    }

    // settings SHOW FPS
    public void Save_ShowFps(bool newValue)
    {
        // Save now
        GetData().ShowDebugHud = newValue;
        GetData().Save();
    }
}
