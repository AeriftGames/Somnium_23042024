using Godot;
using System;

public partial class CPanelSpawns : PanelContainer
{
	public override void _Ready()
	{
        BuildSpawnButtons();
	}

    public void BuildSpawnButtons()
    {
        var allSpawnsInfo = UniversalFunctions.GetAllSpawnObjectsFromDir();
        foreach (var spawn in allSpawnsInfo)
        {
            spawn_object_button spawnButtonInstance = GD.Load<PackedScene>(
                "res://core_systems/debug_hud_system/spawn_object_button.tscn").Instantiate() as spawn_object_button;

            spawnButtonInstance.Text = spawn.name;
            spawnButtonInstance.SetSpawnObjectData(spawn.path, spawn.name);

            VBoxContainer spawnButtonContainer = GetNode<VBoxContainer>("VBoxUp/MarginContainer/VBoxButtons");
            spawnButtonContainer.AddChild(spawnButtonInstance);
        }
    }
}
