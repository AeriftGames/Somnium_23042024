using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

public partial class CLevelLoader : Godot.Object
{
    public struct SLevelInfo
    {
        public string name;
        public string path;
    }

    private GameMaster gm;
    public CLevelLoader(Node ownerInstance)
    {
        gm = (GameMaster)ownerInstance;
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

}
