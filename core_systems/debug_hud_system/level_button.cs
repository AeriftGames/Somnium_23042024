using Godot;
using System;

public partial class level_button : Button
{
	private string level_path = "";
	private string level_name = "";

	public void SetLevelData(string newLevelPath, string newLevelName)
	{
		level_path = newLevelPath;
		level_name = newLevelName;
	}

	public string GetLevelPath()
	{
		return level_path;
	}

    public string GetLevelName()
    {
        return level_name;
    }

    public void _on_pressed()
	{
		GameMaster.GM.LevelLoader.LoadNewWorldLevel(level_path,level_name);
	}
}
