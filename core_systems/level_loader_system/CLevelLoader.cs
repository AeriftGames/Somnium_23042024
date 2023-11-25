using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

public partial class CLevelLoader : Node
{
    public bool isPrecompiledShaders = false;

    private string actualLevelName = (string)ProjectSettings.GetSetting("application/run/main_scene");

    public string loadingScenePath ="";
    Godot.Collections.Array progress;
    bool canUpdate = false;

    public struct SLevelInfo
    {
        public WorldLevel.ELevelType leveltype;
        public string name;
        public string path;
    }

    bool isBenchmark = true;

    [Signal] public delegate void LevelLoadCompleteEventHandler(bool success);

    public void PostInit(bool newIsPrecompiledShaders)
    {
        base._Ready();
        isPrecompiledShaders = newIsPrecompiledShaders;

        // threaded
        progress = new Godot.Collections.Array();
    }

    public void LoadNewWorldLevel(string newLevelScenePath, string newLevelName)
    {
        // Spusti async task pro zmenu levelu
        ChangeLevelSceneWithDelay(newLevelScenePath,newLevelName,1000);
    }

    public async void ChangeLevelSceneWithDelay(string newLevelScenePath, string newLevelName, int delay)
    {
        // zapneme cernou obrazovku
        CGameMaster.GM.GetUniversal().EnableBlackScreen(true);

        // zmenime level
        actualLevelName = newLevelName;

        // potrebny delay
        await Task.Delay(delay);

        // nastavime loading hud
        CGameMaster.GM.GetUniversal().GetLoadingHud().SetInitializeAndVisibleNow(actualLevelName, isPrecompiledShaders);

        // zmenime level
        LoadNewWorldLevel_Threaded(newLevelScenePath, newLevelName);
    }

    public List<SLevelInfo> GetAllLevelsInfo()
    {
        string levels_directory = "res://levels";
        List<SLevelInfo> allLevels = new List<SLevelInfo>();
       
        var a = DirAccess.Open(levels_directory);
        var files = a.GetFiles();
       
        bool is_editor = true;

        string[] levels_files = new string[files.Length];

        // FOR EXPORT
        foreach (string file_name in files) 
        {
            if (file_name.Contains(".tscn.remap"))
            {
                var file_path = a.GetCurrentDir() + "/" + UniversalFunctions.GetStringBetween(file_name,"", ".remap");
                var level_name = UniversalFunctions.GetStringBetween(file_path, a.GetCurrentDir()+"/", ".tscn");

                SLevelInfo level = new SLevelInfo();
                level.path = file_path;
                level.name = level_name;

                //
                if (file_name.Contains("benchmark") || file_name.Contains("Benchmark"))
                    level.leveltype = WorldLevel.ELevelType.BenchmarkLevel;
                else
                    level.leveltype = WorldLevel.ELevelType.GameLevel;

                allLevels.Add(level);

                // for export/editor check
                is_editor = false;
            }
        }

        // FOR EDITOR
        if (is_editor)
        {
            foreach (string file_name in files)
            {
                if (file_name.Contains(".tscn"))
                {
                    var file_path = a.GetCurrentDir() + "/" + file_name;
                    var level_name = UniversalFunctions.GetStringBetween(file_path, a.GetCurrentDir() + "/", ".tscn");

                    SLevelInfo level = new SLevelInfo();
                    level.path = file_path;
                    level.name = level_name;

                    //
                    if (file_name.Contains("benchmark") || file_name.Contains("Benchmark"))
                        level.leveltype = WorldLevel.ELevelType.BenchmarkLevel;
                    else
                        level.leveltype = WorldLevel.ELevelType.GameLevel;

                    allLevels.Add(level);
                }
            }
        }

        if (allLevels.Count == 0) { CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.ERROR, "nenacetli jsme zadne LevelInfo"); }

        return allLevels;
    }

    public void StartPrecompileShaderProcess()
    {
        // TODO - bonus, vytvorit gui scenu pro loading (prekryti vsech tech nesmyslu co se deji za oponou)
        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.INFO, "START PRECOMPILE SHADER PROCESS...");

        Vector3 precompGlobalPosCenter = new Vector3(500, 0, 500);

        FPSCharacter_BasicMoving character_basic = CGameMaster.GM.GetGame().GetFPSCharacter();
        ObjectCamera objectCamera = character_basic.objectCamera;
        character_basic.SetInputEnable(false);
        objectCamera.SetLerpToCharacterEnable(false);

        objectCamera.Camera.GlobalPosition = precompGlobalPosCenter;
        objectCamera.Camera.LookAtFromPosition(precompGlobalPosCenter, character_basic.GlobalPosition);

        // instancovat all_this_shaders scenu
        var all_this_shaders_need_precomp_Instance = (all_this_shaders_need_compiled)GD.Load<PackedScene>(
            "res://core_systems/level_loader_system/all_this_shaders_need_compiled.tscn").Instantiate();
        CGameMaster.GM.GetTree().Root.AddChild(all_this_shaders_need_precomp_Instance);
        all_this_shaders_need_precomp_Instance.GlobalPosition = new Vector3(450, 0, 450);

        // ted se spusti all_this_shader_need_compiled ready funkce a po par vterinach
        // co se dokonci jeji funkce zavola EndPrecompileShaderProcess tady dole.
    }

    public void EndPrecompileShaderProcess()
    {
        CGameMaster.GM.GetSettings().RefreshShaders();

        FPSCharacter_BasicMoving character_basic = CGameMaster.GM.GetGame().GetFPSCharacter();
        ObjectCamera objectCamera = character_basic.objectCamera;

        objectCamera.Camera.GlobalPosition = character_basic.GlobalPosition;
        objectCamera.Camera.GlobalRotation = character_basic.GlobalRotation;
        objectCamera.Camera.Position = Vector3.Zero;
        objectCamera.Camera.Rotation = Vector3.Zero;

        objectCamera.SetLerpToCharacterEnable(true);
        character_basic.SetInputEnable(true);

        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.INFO, "END PRECOMPILE SHADER PROCESS...");
        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.INFO, "GAME START...");

        // loading hud dokonci svoji ulohu a znici se
        CGameMaster.GM.GetUniversal().GetLoadingHud().LoadingIsComplete(false);
    }

    public void SetNewInfoLevelCompilingShader(string newInfo, int process)
    {
        CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.INFO, newInfo);
        CGameMaster.GM.GetUniversal().GetLoadingHud().SetShaderProcessValueText(process.ToString());
    }
    
    public void LoadNewWorldLevel_Threaded(string newLevelScenePath, string newLevelName)
    {
        canUpdate = true;
        loadingScenePath = newLevelScenePath;

        // toto jsem od verze 4.2.rc1 musel nastavit (usethreads) na false, protoze to jinak crashovalo
        ResourceLoader.LoadThreadedRequest(loadingScenePath,"",false);
    }

    public async void Update(double delta)
    {
        if (canUpdate == false) return;

        ResourceLoader.ThreadLoadStatus loadingNewWorldLevelStatus = ResourceLoader.LoadThreadedGetStatus(loadingScenePath, progress);

        // Novy level se nacetl uspesne a lze pouzit
        if (loadingNewWorldLevelStatus == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            canUpdate = false;

            GD.Print("loading scene is complete, it is change now");
            CGameMaster.GM.GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(loadingScenePath));

            await ToSignal(CGameMaster.GM.GetTree(), "process_frame");
            EmitSignal(SignalName.LevelLoadComplete,true);

        }
        else if(loadingNewWorldLevelStatus == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            //GD.Print("loading bar: " + progress[0]);
            CGameMaster.GM.GetUniversal().GetLoadingHud().UpdateProgressBar(((float)progress[0]));
        }
    }

    public WorldLevel GetActualLevelScene()
    {
        WorldLevel level = CGameMaster.GM.GetNode<WorldLevel>("/root/WorldLevel");
        if (level == null)
        {
            // pokud nemuzeme level najit, napiseme chybu do logu
            CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.ERROR,
                "GetActualLevelScene() - Not find /root/WorldLevel");
        }

        return level;
    }

    public string GetActualLevelName() { return actualLevelName; }
}