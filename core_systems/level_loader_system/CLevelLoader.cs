using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

public partial class CLevelLoader : Godot.GodotObject
{
    public bool isPrecompiledShaders = true;

    public string actualLevelName = (string)ProjectSettings.GetSetting("application/run/main_scene");
    public LoadingHud loadingHud = null;
    public bool SDFGI = false;

    //
    public string loadingScenePath ="";
    Godot.Collections.Array progress;
    bool canUpdate = false;

    public struct SLevelInfo
    {
        public string name;
        public string path;
    }

    private GameMaster gm;

    public CLevelLoader(Node ownerInstance, bool newIsPrecompiledShaders)
    {
        gm = (GameMaster)ownerInstance;
        isPrecompiledShaders = newIsPrecompiledShaders;

        // threaded
        progress = new Godot.Collections.Array();
    }

    public async void LoadNewWorldLevel(string newLevelScenePath, string newLevelName)
    {
        // funkce ktera prepne veskera svetla v levelu na hidden
        UnloadLevelProcess();

        // Spusti async task pro zmenu levelu
        await ChangeLevelSceneWithDelay(newLevelScenePath,newLevelName,1000);
    }

    async Task ChangeLevelSceneWithDelay(string newLevelScenePath, string newLevelName, int delay)
    {
        // potrebny delay
        await Task.Delay(delay);

        // zapneme cernou obrazovku
        gm.EnableBlackScreen(true);

        // zmenime level
        actualLevelName = newLevelName;
        //gm.GetTree().ChangeSceneToFile(newLevelScenePath);
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

                    allLevels.Add(level);
                }
            }
        }

        if (allLevels.Count == 0) { GameMaster.GM.Log.WriteLog(gm, LogSystem.ELogMsgType.ERROR, "nenacetli jsme zadne LevelInfo"); }

        return allLevels;
    }

    public void StartPrecompileShaderProcess()
    {
        // vytvorime a nastavime loading hud
        loadingHud = SpawnLoadingHud();
        loadingHud.SetInitializeAndVisibleNow(actualLevelName, false);

        // TODO - bonus, vytvorit gui scenu pro loading (prekryti vsech tech nesmyslu co se deji za oponou)
        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "START PRECOMPILE SHADER PROCESS...");

        Vector3 precompGlobalPosCenter = new Vector3(500, 0, 500);

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
        all_this_shaders_need_precomp_Instance.GlobalPosition = new Vector3(450, 0, 450);

        // ted se spusti all_this_shader_need_compiled ready funkce a po par vterinach
        // co se dokonci jeji funkce zavola EndPrecompileShaderProcess tady dole.
    }

    public async void EndPrecompileShaderProcess()
    {
        GameMaster.GM.GetSettings().RefreshShaders();

        FPSCharacter_BasicMoving character_basic = GameMaster.GM.GetFPSCharacter();
        ObjectCamera objectCamera = character_basic.objectCamera;

        objectCamera.Camera.GlobalPosition = character_basic.GlobalPosition;
        objectCamera.Camera.GlobalRotation = character_basic.GlobalRotation;
        objectCamera.Camera.Position = Vector3.Zero;
        objectCamera.Camera.Rotation = Vector3.Zero;

        objectCamera.SetLerpToCharacterEnable(true);
        character_basic.SetInputEnable(true);

        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "END PRECOMPILE SHADER PROCESS...");
        gm.Log.WriteLog(gm, LogSystem.ELogMsgType.INFO, "GAME START...");

        // loading hud dokonci svoji ulohu a znici se
        loadingHud.LoadingIsComplete(false);

        // Toggle all lights for fix GI
        await SetLevelWorldEnvironment(true);
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

    public void SetNewInfoLevelCompilingShader(string newInfo, int process)
    {
        GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.INFO, newInfo);
        loadingHud.SetShaderProcessValueText(process.ToString());
    }

    public void UnloadLevelProcess()
    {
        // funkce ktera proleze cely level a najde vsechny svetla, ktera nasledne nastavime na visible = false
        // melo by to vyresit problem s GI a prepinani levelu

        GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.INFO, "unload lights");

        Node level = GameMaster.GM.GetNode("/root/worldlevel");
        if (level == null)
        {
            // If worldlevel for spawn dont finded
            GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.ERROR,
                "Not find /root/worldlevel for set unvisible all lights from LevelLoader");
        }
        else
        {
            var allLights = level.FindChildren("", "Light3D", true, false);

            if (allLights.Count > 0)
            {
                GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.INFO, "number of lights: " + allLights.Count);

                foreach (var a in allLights)
                {
                    Light3D light = a as Light3D;
                    if (light != null)
                    {
                        light.Visible = false;
                    }
                }
            }
        }
    }

    public async Task SetLevelWorldEnvironment(bool newSdfgi)
    {
        await Task.Delay(50);
        /*
        Node level = GameMaster.GM.GetNode("/root/worldlevel");
        if (level == null)
        {
            // If worldlevel dosnt finded
            GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.ERROR,
                "Not find /root/worldlevel");
        }
        else
        {
            
            // existuje voxelGI v levelu ? zapneme ho
            VoxelGI b = (VoxelGI)level.FindChild("VoxelGI", false, true);
            if(b!= null)
                b.Visible = true;

            //GameMaster.GM.GetSettings().RefreshShaders();
            
            await Task.Delay(50);

            // prepne mod svetel na disable
            var allLights = level.FindChildren("", "Light3D", true, false);
            if (allLights.Count > 0)
            {
                GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.INFO, "number of lights: " + allLights.Count);

                foreach (var a in allLights)
                {
                    Light3D light = a as Light3D;
                    if (light != null)
                    {
                        // Chceme toggle jen u dynamic lights ! staticke nechceme vypinat !
                        if(light.LightBakeMode != Light3D.BakeMode.Static)
                        {
                            Light3D.BakeMode oldBakeMode = light.LightBakeMode;
                            light.LightBakeMode = Light3D.BakeMode.Disabled;
                            await SetLight3DBakeModeDelay(light, oldBakeMode);
                        }
                    }
                }
            }
        }*/
    }

    private async Task SetLight3DBakeModeDelay(Light3D newLight,Light3D.BakeMode newBakeMode)
    {
        // za 200ms nastav originalni nastaveni bake
        await Task.Delay(10);
        newLight.LightBakeMode = newBakeMode;

        GD.Print(newLight.Name + "set bake mode: " + newBakeMode.ToString() +" path:"+ newLight.GetPath());

        // vypneme cernou obrazovku
        GameMaster.GM.EnableBlackScreen(false);
    }

    public void LoadNewWorldLevel_Threaded(string newLevelScenePath, string newLevelName)
    {
        canUpdate = true;
        loadingScenePath = newLevelScenePath;

        ResourceLoader.LoadThreadedRequest(loadingScenePath,"");
    }

    public async void Update(double delta)
    {
        if (canUpdate == false) return;

        ResourceLoader.ThreadLoadStatus loadingNewWorldLevelStatus = ResourceLoader.LoadThreadedGetStatus(loadingScenePath, progress);

        // Novy level se nacetl uspesne a lze pouzit
        if(loadingNewWorldLevelStatus == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            GD.Print("loading scene is complete, it is change now");
            gm.GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.LoadThreadedGet(loadingScenePath));

            canUpdate = false;
            // Toggle all lights for fix GI
            await SetLevelWorldEnvironment(true);
        }
        else if(loadingNewWorldLevelStatus == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            // Bohuzel nedela co by melo.. ale v tomhle update loopu by jsme mohli treba animovat nejaky loading
            GD.Print("loading bar: " + progress[0]);
        }
    }

    public Node GetActualLevelScene()
    {
        Node level = GameMaster.GM.GetNode("/root/worldlevel");
        if (level == null)
        {
            // pokud nemuzeme level najit, napiseme chybu do logu
            GameMaster.GM.Log.WriteLog(GameMaster.GM, LogSystem.ELogMsgType.ERROR,
                "GetActualLevelScene() - Not find /root/worldlevel");
        }

        return level;
    }
}