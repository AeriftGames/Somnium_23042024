using Godot;
using System;

public partial class FocusActionObject : Node3D
{
	[Export] public bool CanFocusLook = true;
	[Export] public Node3D FocusLookObject = null;

	public Node3D GetFocusMovePoint() { return this; }
	public Node3D GetFocusLookPoint() { return FocusLookObject; }
}
