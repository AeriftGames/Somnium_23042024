using Godot;
using System;

public partial class level_button : Button
{
	private string level_path = "";
	private string level_name = "";
	private WorldLevel.ELevelType levelType;

	public void SetLevelData(string newLevelPath, string newLevelName,WorldLevel.ELevelType newLevelType)
	{
		level_path = newLevelPath;
		level_name = newLevelName;
		levelType = newLevelType;
	}

	public string GetLevelPath() { return level_path; }

    public string GetLevelName()
    {
        return level_name;
    }

    public void _on_pressed()
	{
		if(levelType == WorldLevel.ELevelType.GameLevel)
			GameMaster.GM.GetLevelLoader().LoadNewWorldLevel(level_path,level_name);
		else if(levelType == WorldLevel.ELevelType.BenchmarkLevel)
			GameMaster.GM.GetBenchmarkSystem().StartBenchmarkLevel(level_path,level_name);
	}
}
