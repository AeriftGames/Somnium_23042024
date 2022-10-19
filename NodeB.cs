using Godot;
using System;

public partial class NodeB : Node3D
{
	

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Variant a = GetParent().Call("GetTestText");
		string b = a.AsString();
		a.Dispose();
		GD.Print(b);
	}
}
