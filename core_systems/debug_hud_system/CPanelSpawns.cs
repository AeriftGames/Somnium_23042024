using Godot;
using System;

public partial class CPanelSpawns : CPanelBase
{
    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);

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

            VBoxContainer spawnButtonContainer = GetVBoxElements();
            spawnButtonContainer.AddChild(spawnButtonInstance);
        }
    }
}
