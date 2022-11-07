using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

public partial class CLevelLoader : Godot.Object
{
    public bool isPrecompiledShaders = true;

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

    public void ChangeToNewLevelScene(string newLevelScenePath)
    {
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
        //. odkazat se na player start
        //. potrebujeme se odkazat na kameru hrace - ObjectCamera?
        //. zakazat input hrace
        // zakazat lerpovani kamery k hraci na pozici hlavy
        // nastavit pozici kamery na daleke misto od stredu sceny levelu a smerujici na stred sceny
        // nekam do sceny (na stred nebo pred kameru ?) instancovat scenu se all_this_shaders_need_compiled

        // ve chvili kdy spawneme ll_this_shaders_need_compiled pred kameru, se vsechny shadery zkompiluji
        // pote posleme kameru hrace na puvodni misto na hracove hlave
        // povolime input hrace
        // povolime lerpovani kamery na pozici hlavy
        // zacina hra

        // bonus, vytvorit gui scenu pro loading (prekryti vsech tech nesmyslu co se deji za oponou :D)

        FPSCharacter_BasicMoving character_basic = GameMaster.GM.GetFPSCharacter();
        ObjectCamera objectCamera = character_basic.objectCamera;
        character_basic.SetInputEnable(false);
        objectCamera.SetLerpToCharacterEnable(false);

        var camRot = objectCamera.Camera.Rotation;
        var camPos = objectCamera.Camera.Position;


    }

}
