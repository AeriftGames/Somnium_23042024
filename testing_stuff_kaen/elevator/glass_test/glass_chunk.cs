using Godot;
using System;
using System.Threading.Tasks;

public partial class glass_chunk : RigidBody3D
{
    public override void _Ready()
    {
        base._Ready();

        Sleeping = true;

        DelayStart();
    }

    public async void DelayStart()
    {
        Sleeping = false;
        Sleeping = true;
        await Task.Delay(5000);
        Sleeping = true;
        Freeze = false;
        Sleeping = true;
    }
}
