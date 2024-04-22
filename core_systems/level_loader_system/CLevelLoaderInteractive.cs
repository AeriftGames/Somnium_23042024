using Godot;
using System;

public partial class CLevelLoaderInteractive : Godot.GodotObject
{
    private CGameMaster gm;
    public string actualLevelName = (string)ProjectSettings.GetSetting("application/run/main_scene");
    public struct SLevelInfo
    {
        public string name;
        public string path;
    }

    public CLevelLoaderInteractive(Node ownerInstance)
    {
        gm = (CGameMaster)ownerInstance;
    }

    public void LoadNewWorldLevel(string newLevelScenePath, string newLevelName)
    {
        ResourceLoader.LoadThreadedRequest(newLevelScenePath);
    }

    public void Update(double delta)
    {

    }
}
