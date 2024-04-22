using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public partial class UniversalFunctions : Node
{
    public bool test = false;

    public struct HitResult
    {
        public bool isHit;
        public Vector3 HitPosition;
        public Vector3 HitNormal;
        public Node HitNode;
    }

    public struct SLevelData
    {
        public string name;
        public string path;
    }

    public struct SFpsInfo
    {
        public int minFps;
        public int maxFps;
        public int avgFps;
    }

    static public SFpsInfo CalculateFpsInfo(Array<int>newAllFpsData)
    {
        SFpsInfo FpsInfo = new SFpsInfo();

        int minFps = 10000;     // pro zacatek musi byt nejvyssi
        int maxFps = 0;         // pro zacatek musi byt nejmensi
        int avgFps;
        int w_avgFps = 0;

        // kalkulace min max avg FPS
        foreach (int fps in newAllFpsData)
        {
            if (fps < minFps && fps > 10)
                minFps = fps;

            if (fps > maxFps)
                maxFps = fps;

            w_avgFps = w_avgFps + fps;
        }

        avgFps = w_avgFps / newAllFpsData.Count;
        GD.Print(avgFps);

        FpsInfo.minFps = minFps;
        FpsInfo.maxFps = maxFps;
        FpsInfo.avgFps = avgFps;

        return FpsInfo;
    }

    static public HitResult IsSimpleRaycastHit(Node3D owner, Vector3 from, Vector3 to, uint collisionMask)
    {
        HitResult result = new HitResult();

        PhysicsDirectSpaceState3D directSpace = owner.GetWorld3D().DirectSpaceState;
        if(directSpace != null)
        {
            PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
            rayParam.From = from;
            rayParam.To = to;
            rayParam.CollisionMask = collisionMask;

            var rayResult = directSpace.IntersectRay(rayParam);
            if (rayResult.Count > 0)
            {
                result.isHit = true;
                result.HitPosition = (Vector3)rayResult["position"];
                result.HitNormal = (Vector3)rayResult["normal"];
                result.HitNode = (Node)rayResult["collider"];
            }
            else
            {
                result.isHit = false;
                result.HitPosition = Vector3.Zero;
                result.HitNormal = Vector3.Zero;
                result.HitNode = null;
            }
        }

        return result;
    }
    public static string[] GetDirectoryFiles(string directory, string filetype)
    {
        string[] a = Directory.GetFiles(Directory.GetCurrentDirectory() + directory, "*" + filetype);
        if (a == null) return null;
        if (a.Length == 0) return null;
        
        return a;
    }
    public static string GetStringBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }
    public static void PlayRandomSound(AudioStreamPlayer audioPlayer, Array<AudioStream> audioStreams, float volumeDB, float pitch)
    {
        if (audioPlayer == null) return;
        if (audioStreams.Count < 1) return;

        // random pick sound from array and play it
        RandomNumberGenerator random = new RandomNumberGenerator();
        int id = 0;

        // 20 chances
        for (int i = 0; i < 20; i++)
        {
            // randomize sound id from array
            random.Randomize();
            id = random.RandiRange(0, audioStreams.Count - 1);

            // if is not same, break for loop
            if (audioPlayer.Stream != audioStreams[id])
                break;
        }

        // play sounds
        audioPlayer.VolumeDb = volumeDB;
        audioPlayer.PitchScale = pitch;
        audioPlayer.Stream = audioStreams[id];
        audioPlayer.Play();

        //GD.Print(audioStreams[id].ResourcePath);
    }
    public static void PlayRandom3DSound(AudioStreamPlayer3D audioPlayer, Array<AudioStream> audioStreams, float volumeDB, float pitch)
    {
        if (audioPlayer == null) return;
        if (audioStreams.Count < 1) return;

        // random pick sound from array and play it
        RandomNumberGenerator random = new RandomNumberGenerator();
        int id = 0;

        // 20 chances
        for (int i = 0; i < 20; i++)
        {
            // randomize sound id from array
            random.Randomize();
            id = random.RandiRange(0, audioStreams.Count - 1);

            // if is not same, break for loop
            if (audioPlayer.Stream != audioStreams[id])
                break;
        }

        // play sounds
        audioPlayer.VolumeDb = volumeDB;
        audioPlayer.PitchScale = pitch;
        audioPlayer.Stream = audioStreams[id];
        audioPlayer.Play();

        //GD.Print(audioStreams[id].ResourcePath);
    }
    public static Vector3 GetNodeForwardVector(Node3D node3D)
    {
        return node3D.GlobalTransform.Basis.Z.Normalized();
    }
    public static Vector3 GetNodeRightVector(Node3D node3D)
    {
        return node3D.GlobalTransform.Basis.X.Normalized();
    }
    public static Vector3 GetNodeUpVector(Node3D node3D)
    {
        return node3D.GlobalTransform.Basis.Y.Normalized();
    }
    public static Godot.Collections.Array<Node3D> SpawnGameObjectToWorld(Node spawnParent,string path,Vector3 spawnPosition,int amountOfSpawn = 1)
    {
        Godot.Collections.Array<Node3D> allSpawnNodes = new Godot.Collections.Array<Node3D>();

        var spawnPackedScene = GD.Load<PackedScene>(path);
        Node3D spawnNode = null;

        for (int i = 0; i < amountOfSpawn; i++)
        {
            spawnNode = spawnPackedScene.Instantiate() as Node3D;
            spawnParent.AddChild(spawnNode);
            spawnNode.GlobalPosition = spawnPosition;
            allSpawnNodes.Add(spawnNode);
        }

        return allSpawnNodes;
    }
    public static List<SLevelData> GetAllSpawnObjectsFromDir(string newDir = "res://spawn")
    {
        string spawn_directory = newDir;
        List<SLevelData> allSpawnObjects = new List<SLevelData>();

        var a = DirAccess.Open(spawn_directory);
        var files = a.GetFiles();

        bool is_editor = true;

        string[] levels_files = new string[files.Length];

        // FOR EXPORT
        foreach (string file_name in files)
        {
            if (file_name.Contains(".tscn.remap"))
            {
                var file_path = a.GetCurrentDir() + "/" + UniversalFunctions.GetStringBetween(file_name, "", ".remap");
                var spawn_name = UniversalFunctions.GetStringBetween(file_path, a.GetCurrentDir() + "/", ".tscn");

                SLevelData newSpawnObjectInfo = new SLevelData();
                newSpawnObjectInfo.path = file_path;
                newSpawnObjectInfo.name = spawn_name;

                allSpawnObjects.Add(newSpawnObjectInfo);

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
                    var spawn_name = UniversalFunctions.GetStringBetween(file_path, a.GetCurrentDir() + "/", ".tscn");

                    SLevelData newSpawnObjectInfo= new SLevelData();
                    newSpawnObjectInfo.path = file_path;
                    newSpawnObjectInfo.name = spawn_name;

                    allSpawnObjects.Add(newSpawnObjectInfo);
                }
            }
        }

        if (allSpawnObjects.Count == 0) { CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.ERROR, "nenacetli jsme zadne LevelInfo"); }

        return allSpawnObjects;
    }

    static public string GetQualityLevelText(int newQualityLevelID)
    {
        // vyber textu aktualni kvality
        switch (newQualityLevelID)
        {
            case 0: return "lowest quality";
            case 1: return "low quality";
            case 2: return "medium quality";
            case 3: return "high quality";
            case 4: return "highest quality";
            default: return "none";
        }
    }

    static public levelinfo_base_resource GetLevelInfoData(Resource newlevelInfo)
    {
        if (newlevelInfo == null) return null;

        Resource data = GD.Load(newlevelInfo.ResourcePath);
        if (data != null && data is levelinfo_base_resource leveldata)
            return leveldata;
        else
            return null;
    }
    
    static public levelinfo_base_resource GetLevelInfoData(string new_levelInfopath)
    {
        Resource data = GD.Load(new_levelInfopath);
        if (data != null && data is levelinfo_base_resource leveldata)
            return leveldata;
        else
            return null;
    }
    
    
    public static List<levelinfo_base_resource> GetAllLevelInfoDataFromDir(
        string newDir = "res://levels/all_levels_info_resources/game_levels/")
    {
        string directory = newDir;
        List<levelinfo_base_resource> AllLevelInfo = new List<levelinfo_base_resource>();

        var a = DirAccess.Open(directory);
        var files = a.GetFiles();

        bool is_editor = true;

        string[] levels_files = new string[files.Length];

        // FOR EXPORT
        foreach (string file_name in files)
        {
            if (file_name.Contains(".tres.remap"))
            {
                string file_path = a.GetCurrentDir() + "/" + UniversalFunctions.GetStringBetween(file_name, "", ".remap");

                levelinfo_base_resource newLevelInfo = GetLevelInfoData(file_path);
                AllLevelInfo.Add(newLevelInfo);

                // for export/editor check
                is_editor = false;
            }
        }

        // FOR EDITOR
        if (is_editor)
        {
            foreach (string file_name in files)
            {
                if (file_name.Contains(".tres"))
                {
                    string file_path = a.GetCurrentDir() + "/" + file_name;

                    levelinfo_base_resource newLevelInfo = GetLevelInfoData(file_path);
                    AllLevelInfo.Add(newLevelInfo);
                }
            }
        }

        if (AllLevelInfo.Count == 0) { CGameMaster.GM.GetUniversal().GetMasterLog().WriteLog(CGameMaster.GM, CMasterLog.ELogMsgType.ERROR, "nenacetli jsme zadne LevelInfo"); }

        return AllLevelInfo;
    }

    public static string DetectSurfaceMaterialOfFloor(Node3D newCaller,Vector3 newPos)
    {
        PhysicsDirectSpaceState3D directSpace = newCaller.GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D rayParam = new PhysicsRayQueryParameters3D();
        rayParam.From = newPos;
        rayParam.To = newPos + (Vector3.Down * 1);

        var rayResult = directSpace.IntersectRay(rayParam);
        if (rayResult.Count > 0)
        {
            Node HitCollider = (Node)rayResult["collider"];
            if (HitCollider == null) return "none";

            if (HitCollider.IsInGroup("material_surface_metal"))
                return "material_surface_metal";

            if (HitCollider.IsInGroup("material_surface_wood"))
                return "material_surface_wood";
        }
        return "none";
    }
    /*
    public static void TryCall(Node newObject,StringName newFunction, params Variant[] args = null)
    {
        if (newObject == null)
        {
            // Log error dont exist object
        }
        else if (newObject.HasMethod(newFunction) == false)
        {
            // Log error has method
            GD.Print("neexistuje methoda");
        }
        else
        {
            newObject.Call(newFunction, args);
        }
    }
    */
    /*
    public static void TryCall(Node newObject, StringName newFunction)
    {
        if (newObject == null)
        {
            // Log error dont exist object
        }
        else if (newObject.HasMethod(newFunction) == false)
        {
            // Log error has method
            GD.Print("neexistuje methoda");
        }
        else
        {
            newObject.Call(newFunction);
        }
    }
    */
    public static void TryCall(Node newObject, StringName newFunction)
    {
        if (newObject == null)
        {
            // Log error dont exist object
        }
        else if (newObject.HasMethod(newFunction) == false)
        {
            // Log error has method
            GD.Print("neexistuje methoda");
        }
        else
        {
            newObject.Call(newFunction);
        }
    }

    // for getting vram in mb
    public static int GetVRamUsageInMBytes()
    {
        int a = (int) RenderingServer.GetRenderingInfo(RenderingServer.RenderingInfo.VideoMemUsed);
        a = a / 1000000;
        return a;
    }
}