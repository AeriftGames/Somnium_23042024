using Godot;
using System;

public partial class level_button : Button
{
	private string level_path = "";

	public void SetLevelPath(string newLevelPath)
	{
		level_path = newLevelPath;
	}

	public string GetLevelPath()
	{
		return level_path;
	}

    public void _on_pressed()
	{
		GameMaster.GM.LevelLoader.ChangeToNewLevelScene(level_path);
	}
}
