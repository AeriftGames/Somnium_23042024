using Godot;
using System;

public partial class LoadingHud : Control
{
	public override void _Ready()
	{
		// nastavime sebe do gamemastera
		GameMaster.GM.SetLoadingHud(this);
	}

	public override void _Process(double delta)
	{
	}


}
