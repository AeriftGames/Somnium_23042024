using Godot;
using System;
using System.Threading.Tasks;
using System.Windows;

public partial class global_settings : Godot.GodotObject
{
    private GameMaster gm;

    // promene ktere slouzi pro otestovani aktualniho stavu a nejde to jinak
    private bool actual_halfResolutionGI = false;

    public global_settings(Node ownerInstance)
    {
        gm = (GameMaster)ownerInstance;

        // Audio volumes init
        LoadAndApply_AllAudioSettings();
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

    // Spoustime v player startu pri startu hry (aby se mohl nastavit WorldEnvironment)
    public void LoadAndApply_AllGraphicsSettings()
    {
        // nacteme veskera data ulozena ze souboru
        global_settings_data data = GetData();

        // pouze aplikujeme jednotliva nastaveni = neukladame do souboru
        Apply_ScreenMode(data.ScreenMode, true, false);
        Apply_ScreenSizeID(data.ScreenSizeID, true, false);
        Apply_Ssao(data.Ssao, true, false);
        Apply_Ssil(data.Ssil, true, false);
        Apply_Scale3D(data.Scale3d, true, false);
        Apply_HalfResolutionGI(data.HalfResolutionGI, true, false);
        Apply_AntialiasID(data.AntialiasID, true, false);
        Apply_GlobalIlumination(data.GlobalIlumination, true, false);
        Apply_UnlockMaxFps(data.UnlockMaxFps, true, false);
        Apply_DisableVsync(data.DisableVsync, true, false);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all graphics data is apply");
    }

    // Toto volani projede veskera aktualni aplikovana graficka nastaveni a
    // ulozi je do souboru global_settings_data.tres
    public void SaveActual_AllGraphicsSettings()
    {
        global_settings_data data = GetData();

        // neaplikujeme, pouze ulozime jednotliva aktualni nastaveni do souboru
        Apply_ScreenMode(GetActual_ScreenMode(), false, true);
        Apply_ScreenSizeID(GetActual_ScreenSizeID(), false, true);
        Apply_Ssao(GetActual_Ssao(), false, true);
        Apply_Ssil(GetActual_Ssil(), false, true);
        Apply_GlobalIlumination(GetActual_GlobalIlumination(), false, true);
        Apply_AntialiasID(GetActual_AntialiasID(), false, true);
        Apply_Scale3D(GetActual_Scale3D(), false, true);
        Apply_HalfResolutionGI(GetActual_HalfResolutionGI(), false, true);
        Apply_UnlockMaxFps(GetActual_UnlockMaxFps(), false, true);
        Apply_DisableVsync(GetActual_DisableVsync(), false, true);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all graphics data is saved");
    }

    // Spoustime zde v global_settings pri konstrukci (init)
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

    // Spoustime zde v global_settings pri konstrukci (init)
    public void LoadAndApply_AllInputsSettings()
    {
        // nacteme veskera data ulozena ze souboru
        global_settings_data data = GetData();

        // pouze aplikujeme jednotliva nastaveni = neukladame do souboru
        Apply_LookMouseSmooth(data.LookMouseSmooth, true, false);
        Apply_LookMouseSensitivity(data.LookMouseSensitivity, true, false);
        Apply_LookGamepadSmooth(data.LookGamepadSmooth, true, false);
        Apply_LookGamepadSensitivity(data.LookGamepadSensitivity, true, false);
        Apply_InverseVerticalLook(data.InverseVerticalLook, true, false);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all inputs data is apply");
    }

    // Toto volani projede veskera aktualni aplikovana inputs nastaveni a
    // ulozi je do souboru global_settings_data.tres
    public void SaveActual_AllInputsSettings()
    {
        // nacteme veskera data ulozena ze souboru
        global_settings_data data = GetData();

        // pouze aplikujeme jednotliva nastaveni = neukladame do souboru
        Apply_LookMouseSmooth(GetActual_LookMouseSmooth(), false, true);
        Apply_LookMouseSensitivity(GetActual_LookMouseSensitivity(), false, true);
        Apply_LookGamepadSmooth(GetActual_LookGamepadSmooth(), false, true);
        Apply_LookGamepadSensitivity(GetActual_LookGamepadSensitivity(), false, true);
        Apply_InverseVerticalLook(GetActual_InverseVerticalLook(), false, true);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "all inputs data is saved");
    }

    /**************************************************************************/
    // GRAPHICS

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
                gm.GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;

                // impulz pro zmenu rozliseni
                if (GetActual_ScreenSizeID() == 2)
                    Apply_ScreenSizeID(2, true, false);

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
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "chybne vraceni hodnoty z GetActual_ScreenMode," +
                " nejspis je jiny nastaveni screen mode nez dane id presety");
            return -1;
        }
    }

    // settings WINDOW SIZE ID
    public void Apply_ScreenSizeID(int newScreenSizeID, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (newScreenSizeID == 0)
            {
                //1280x720
                gm.GetTree().Root.Size = new Vector2I(1280, 720);
                //Apply_ScreenMode(GetActual_ScreenMode(),true,false);
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: screen size id = 1280x720");

            }
            else if (newScreenSizeID == 1)
            {
                //1920x1080
                gm.GetTree().Root.Size = new Vector2I(1920, 1080);
                //Apply_ScreenMode(GetActual_ScreenMode(), true, false);
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: screen size id = 1920x1080");
            }
            else if (newScreenSizeID == 2)
            {
                // screen size
                //if(GetActual_ScreenMode() == 0)
                gm.GetTree().Root.Size = new Vector2I(DisplayServer.ScreenGetSize().X - 1, DisplayServer.ScreenGetSize().Y - 1);

                //Apply_ScreenMode(0, true, false);
                //gm.GetTree().Root.Size = new Vector2i(DisplayServer.ScreenGetSize().x-1, DisplayServer.ScreenGetSize().y-1);
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: screen size id = screen size");
            }
            else
            {
                //chyba - mimo rozsah id
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "pokus o chybne nastaveni screen size id," +
                    " hodnotou: " + newScreenSizeID);
            }
        }

        // Save now
        if (newSaveNow)
        {
            GetData().ScreenSizeID = newScreenSizeID;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: screen size id = " + newScreenSizeID);
        }
    }

    public int GetActual_ScreenSizeID()
    {
        if (gm.GetTree().Root.Size.X == 1280 && gm.GetTree().Root.Size.Y == 720)
        {
            //1280x720
            return 0;
        }
        else if (gm.GetTree().Root.Size.X == 1920 && gm.GetTree().Root.Size.Y == 1080)
        {
            //1920x1080
            return 1;
        }
        else if (gm.GetTree().Root.Mode == Window.ModeEnum.ExclusiveFullscreen ||
            gm.GetTree().Root.Mode == Window.ModeEnum.Windowed)
        {
            return 2;
        }
        else
        {
            //error - mimo rozsah id
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "chybne vraceni hodnoty z GetActual_ScreenSizeID," +
                " nejspis je jiny nastaveni screen size nez dane id presety");
            return -1;
        }
    }

    // Aplikuje ruzna GI
    public void Apply_GlobalIlumination(int newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        if (newApplyNow)
        {
            if (newValue == 0)
            {
                // disable
                VoxelGI voxelGI = gm.LevelLoader.GetActualLevelScene().GetNode<VoxelGI>("VoxelGI");
                if (voxelGI != null)
                    voxelGI.Visible = false;

                Apply_Sdfgi(false, true, false);
                gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: gi = " + newValue + "(disable)");
            }
            else if (newValue == 1)
            {
                // voxel
                Apply_Sdfgi(false, true, false);
                VoxelGI voxelGI = gm.LevelLoader.GetActualLevelScene().GetNode<VoxelGI>("VoxelGI");
                if (voxelGI != null)
                    voxelGI.Visible = true;

                // mozny fix na zmenu to voxel kdy extremne zacne svitit fog
                // vypada to ze staci prenacist shadery a na to staci zmena screenmode - aktualne asi funguje bez toho
                //int a = GetActual_ScreenMode();
                //Apply_ScreenMode(0);
                //Apply_ScreenMode(a);
                gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: gi = " + newValue + "(voxel)");
            }
            else if (newValue == 2)
            {
                // sdfgi
                VoxelGI voxelGI = gm.LevelLoader.GetActualLevelScene().GetNode<VoxelGI>("VoxelGI");
                if (voxelGI != null)
                    voxelGI.Visible = false;
                Apply_Sdfgi(true, true, false);
                gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: gi = " + newValue + "(sdfgi)");
            }
        }

        if (newSaveNow)
        {
            GetData().GlobalIlumination = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: gi = " + newValue);
        }

        RefreshShaders();
    }

    public int GetActual_GlobalIlumination()
    {
        // 0 - disable,1 - voxel, 2 - sdfgi
        int a = 0;
        VoxelGI voxelGI = gm.LevelLoader.GetActualLevelScene().GetNode<VoxelGI>("VoxelGI");
        if (voxelGI != null)
            if (voxelGI.Visible)
                a = 1;

        if (GetActual_Sdfgi())
            a = 2;

        return a;
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

    // settings SDFGI = nepouzivat z venku, misto toho pouzit Apply_GlobalIlumination()
    private void Apply_Sdfgi(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
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

    // settings UNLOCK MAX FPS
    public void Apply_UnlockMaxFps(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (newValue == false)
                Engine.MaxFps = 60;
            else
                Engine.MaxFps = 0;

            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: unlock max fps = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().UnlockMaxFps = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: unlock max fps = " + newValue);
        }
    }

    public bool GetActual_UnlockMaxFps()
    {
        if (Engine.MaxFps == 60)
            return false;
        else
            return true;
    }

    // settings DISABLE VSYNC
    public void Apply_DisableVsync(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (newValue == false)
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
            else
                DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);

            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: disable vsync = " + newValue);
        }

        // Save now
        if (newSaveNow)
        {
            GetData().UnlockMaxFps = newValue;
            GetData().Save();
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "save video settings: disable vsync = " + newValue);
        }
    }

    public bool GetActual_DisableVsync()
    {
        if (DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled)
            return false;
        else
            return true;
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
                gm.GetTree().Root.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3D = Viewport.Msaa.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = disable");

            }
            else if (newAntialiasID == 1)
            {
                //only ss_aa
                gm.GetTree().Root.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3D = Viewport.Msaa.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = only ss_aa");
            }
            else if (newAntialiasID == 2)
            {
                //ss_aa+taa
                gm.GetTree().Root.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3D = Viewport.Msaa.Disabled;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = ss_aa + taa");
            }
            else if (newAntialiasID == 3)
            {
                //only msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Disabled;
                gm.GetTree().Root.UseTaa = false;
                gm.GetTree().Root.Msaa3D = Viewport.Msaa.Msaa2X;
                GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: antialias id = only msaa3d_2x");
            }
            else if (newAntialiasID == 4)
            {
                //ss_aa+taa+msaa3d_2x
                gm.GetTree().Root.ScreenSpaceAA = Viewport.ScreenSpaceAAEnum.Fxaa;
                gm.GetTree().Root.UseTaa = true;
                gm.GetTree().Root.Msaa3D = Viewport.Msaa.Msaa2X;
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
        if (gm.GetTree().Root.ScreenSpaceAA == Viewport.ScreenSpaceAAEnum.Disabled &&
            gm.GetTree().Root.UseTaa == false &&
            gm.GetTree().Root.Msaa3D == Viewport.Msaa.Disabled)
        {
            //disable
            return 0;
        }
        else if (gm.GetTree().Root.ScreenSpaceAA == Viewport.ScreenSpaceAAEnum.Fxaa &&
            gm.GetTree().Root.UseTaa == false &&
            gm.GetTree().Root.Msaa3D == Viewport.Msaa.Disabled)
        {
            //ss_aa
            return 1;
        }
        else if (gm.GetTree().Root.ScreenSpaceAA == Viewport.ScreenSpaceAAEnum.Fxaa &&
            gm.GetTree().Root.UseTaa == true &&
            gm.GetTree().Root.Msaa3D == Viewport.Msaa.Disabled)
        {
            //ss_aa+taa
            return 2;
        }
        else if (gm.GetTree().Root.ScreenSpaceAA == Viewport.ScreenSpaceAAEnum.Disabled &&
            gm.GetTree().Root.UseTaa == false &&
            gm.GetTree().Root.Msaa3D == Viewport.Msaa.Msaa2X)
        {
            //only msaa3d_2x
            return 3;
        }
        else if (gm.GetTree().Root.ScreenSpaceAA == Viewport.ScreenSpaceAAEnum.Fxaa &&
            gm.GetTree().Root.UseTaa == true &&
            gm.GetTree().Root.Msaa3D == Viewport.Msaa.Msaa2X)
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

    // settings SCALE 3D
    public void Apply_Scale3D(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if(newApplyNow)
        {
            gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "apply video settings: scale 3d = " + newValue);
            gm.GetTree().Root.Scaling3DScale = newValue;
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
        return gm.GetTree().Root.Scaling3DScale;
    }

    // settings HALF RESOLUTION GI
    public void Apply_HalfResolutionGI(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            RenderingServer.GISetUseHalfResolution(newValue);
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

    /**************************************************************************/
    // INPUTS

    // settings Mouse Smooth
    public void Apply_LookMouseSmooth(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (GameMaster.GM.GetFPSCharacter() == null) return;
                GameMaster.GM.GetFPSCharacter().MouseSmooth = newValue;
        }

        // Save nowj
        if (newSaveNow)
        {
            GetData().LookMouseSmooth = newValue;
            GetData().Save();
        }
    }

    public float GetActual_LookMouseSmooth()
    {
        if (GameMaster.GM.GetFPSCharacter() == null) return 1f;
        return GameMaster.GM.GetFPSCharacter().MouseSmooth;
    }

    // settings Mouse Sensitivity
    public void Apply_LookMouseSensitivity(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (GameMaster.GM.GetFPSCharacter() == null) return;
            GameMaster.GM.GetFPSCharacter().MouseSensitivity = newValue;
        }

        // Save nowj
        if (newSaveNow)
        {
            GetData().LookMouseSensitivity = newValue;
            GetData().Save();
        }
    }

    public float GetActual_LookMouseSensitivity()
    {
        if (GameMaster.GM.GetFPSCharacter() == null) return 1f;
        return GameMaster.GM.GetFPSCharacter().MouseSensitivity;
    }

    // settings Gamepad Smooth
    public void Apply_LookGamepadSmooth(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (GameMaster.GM.GetFPSCharacter() == null) return;
            GameMaster.GM.GetFPSCharacter().GamepadSmooth = newValue;
        }

        // Save nowj
        if (newSaveNow)
        {
            GetData().LookGamepadSmooth = newValue;
            GetData().Save();
        }
    }

    public float GetActual_LookGamepadSmooth()
    {
        if (GameMaster.GM.GetFPSCharacter() == null) return 1f;
        return GameMaster.GM.GetFPSCharacter().GamepadSmooth;
    }

    // settings Gamepad Sensitivity
    public void Apply_LookGamepadSensitivity(float newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (GameMaster.GM.GetFPSCharacter() == null) return;
            GameMaster.GM.GetFPSCharacter().GamepadSensitvity = newValue;
        }

        // Save nowj
        if (newSaveNow)
        {
            GetData().LookGamepadSensitivity = newValue;
            GetData().Save();
        }
    }

    public float GetActual_LookGamepadSensitivity()
    {
        if (GameMaster.GM.GetFPSCharacter() == null) return 1f;
        return GameMaster.GM.GetFPSCharacter().GamepadSensitvity;
    }

    // settings InverseVerticalLook
    public void Apply_InverseVerticalLook(bool newValue, bool newApplyNow = false, bool newSaveNow = false)
    {
        // Apply now
        if (newApplyNow)
        {
            if (GameMaster.GM.GetFPSCharacter() == null) return;
            GameMaster.GM.GetFPSCharacter().InverseVerticalLook = newValue;
        }

        // Save nowj
        if (newSaveNow)
        {
            GetData().InverseVerticalLook = newValue;
            GetData().Save();
        }
    }

    public bool GetActual_InverseVerticalLook()
    {
        if (GameMaster.GM.GetFPSCharacter() == null) return false;
        return GameMaster.GM.GetFPSCharacter().InverseVerticalLook;
    }

    /**************************************************************************/
    // OTHERS

    // refresh antialising (working as impulse refresh shaders)
    public async void RefreshShaders()
    {
        await RefreshShadersBackTask(GetActual_AntialiasID());
        Apply_AntialiasID(0,true,false);
    }

    private async Task RefreshShadersBackTask(int value)
    {
        await Task.Delay(100);
        Apply_AntialiasID(value,true,false);
    }
}
