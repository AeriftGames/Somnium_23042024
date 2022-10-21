using Godot;
using System;
public partial class game_master : Node
{
    public static game_master GM;

    public override void _Ready()
    {
        GD.Print("GameMaster loaded");
        GM = this;
    }

    public override void _Process(double delta)
    {
    }
}
