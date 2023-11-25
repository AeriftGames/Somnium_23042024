using Godot;
using System;
using System.Threading.Tasks;

public partial class test_level : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        PostStart();
    }

	public async void PostStart()
	{
        await Task.Delay(200);
        CGameMaster.GM.GetUniversal().EnableBlackScreen(false);
    }

}
