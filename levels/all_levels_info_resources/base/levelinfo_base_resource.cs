using Godot;
using System;

public partial class levelinfo_base_resource : Resource
{
    [Export] public WorldLevel.ELevelType LevelType { get; set; }
    [Export] public string LevelPath { get; set; }
    [Export] public string LevelName { get; set; }
}
