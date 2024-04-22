using Godot;
using System;

public partial class CPanelSpawns : CPanelBase
{
    private Label LabelNum = null;

    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);

        LabelNum = GetNode<Label>("%LabelNum");

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

    public void _on_h_slider_value_changed(float newValue)
    {
        LabelNum.Text = newValue.ToString();
    }

    public int GetNumSpawnObject() { return (int) GetNode<HSlider>("%HSliderSpawnNum").Value; }
}
