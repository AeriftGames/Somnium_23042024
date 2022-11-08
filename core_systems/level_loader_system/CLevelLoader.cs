using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

public partial class CLevelLoader : Godot.Object
{
    public bool isPrecompiledShaders = true;

    public string actualLevelName = (string)ProjectSettings.GetSetting("application/run/main_scene");
    public LoadingHud loadingHud = null;

    public struct SLevelInfo
    {
        public string name;
        public string path;
    }

    private GameMaster gm;
    public CLevelLoader(Node ownerInstance, bool isPrecompiledShaders)
    {
        gm = (GameMaster)ownerInstance;
        this.isPrecompiledShaders = isPrecompiledShaders;
    }

    public void LoadNewWorldLevel(string newLevelScenePath, string newLevelName)
    {
        actualLevelName = newLevelName;
        gm.GetTree().ChangeSceneToFile(newLevelScenePath);
    }

    public List<SLevelInfo> GetAllLevelsInfo()
    {
        string levels_directory = "\\levels";
        List<SLevelInfo> allLevels = new List<SLevelInfo>();

        string[] allFiles = UniversalFunctions.GetDirectoryFiles(levels_directory, ".tscn");
        foreach (string file_path in allFiles)
        {
            var file_name = UniversalFunctions.GetStringBetween(file_path, Directory.GetCurrentDirectory() + 
                levels_directory+"\\", ".tscn");

            SLevelInfo level = new SLevelInfo();
            level.path = file_path;
            level.name = file_name;

            allLevels.Add(level);
        }

        if(allLevels.Count == 0) { GameMaster.GM.Log.WriteLog(gm,LogSystem.ELogMsgType.ERROR,"nenacetli jsme zadne LevelInfo"); }

        return allLevels;
    }

    public void StartPrecompileShaderProcess()
    {
        // vytvorime a nastavime loading hud
        loadingHud = SpawnLoadingHud();
        loadingHud.SetInitializeAndVisibleNow(actualLevelName, false);

        // TODO - bonus, vytvorit gui scenu pro loading (prekryti vsech tech nesmyslu co se deji za oponou :D)
        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "START PRECOMPILE SHADER PROCESS...");

        Vector3 precompGlobalPosCenter = new Vector3(500,0,500);

        FPSCharacter_BasicMoving character_basic = GameMaster.GM.GetFPSCharacter();
        ObjectCamera objectCamera = character_basic.objectCamera;
        character_basic.SetInputEnable(false);
        objectCamera.SetLerpToCharacterEnable(false);

        objectCamera.Camera.GlobalPosition = precompGlobalPosCenter;
        objectCamera.Camera.LookAtFromPosition(precompGlobalPosCenter, character_basic.GlobalPosition);

        // instancovat all_this_shaders scenu
        var all_this_shaders_need_precomp_Instance = (all_this_shaders_need_compiled)GD.Load<PackedScene>(
            "res://core_systems/level_loader_system/all_this_shaders_need_compiled.tscn").Instantiate();
        gm.GetTree().Root.AddChild(all_this_shaders_need_precomp_Instance);
        all_this_shaders_need_precomp_Instance.GlobalPosition = new Vector3(450,0,450);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "END PRECOMPILE SHADER PROCESS...");
        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "GAME START...");

        // ted se spusti all_this_shader_need_compiled ready funkce a po par vterinach
        // co se dokonci jeji funkce zavola EndPrecompileShaderProcess tady dole.
    }

    public void EndPrecompileShaderProcess()
    {
        FPSCharacter_BasicMoving character_basic = GameMaster.GM.GetFPSCharacter();
        ObjectCamera objectCamera = character_basic.objectCamera;

        objectCamera.Camera.GlobalPosition = character_basic.GlobalPosition;
        objectCamera.Camera.GlobalRotation = character_basic.GlobalRotation;
        objectCamera.Camera.Position = Vector3.Zero;
        objectCamera.Camera.Rotation = Vector3.Zero;

        objectCamera.SetLerpToCharacterEnable(true);
        character_basic.SetInputEnable(true);

        // loading hud dokonci svoji ulohu a znici se
        loadingHud.LoadingIsComplete(false);
    }

    // instantiate a addchild loading hud to fpscharacter/allhuds and return it
    private LoadingHud SpawnLoadingHud()
    {
        // instancovat all_this_shaders scenu
        var loading_hud_Instance = (LoadingHud)GD.Load<PackedScene>(
            "res://core_systems/level_loader_system/loading_hud.tscn").Instantiate();

        gm.GetFPSCharacter().GetAllHudsControlNode().AddChild(loading_hud_Instance);

        return loading_hud_Instance;
    }

}
