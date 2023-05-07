using Godot;
using System;
using System.ComponentModel;

public partial class ItemPreview : SubViewportContainer
{
	MeshInstance3D itemMeshPreview = null;
	public override void _Ready()
	{
		itemMeshPreview = GetNode<MeshInstance3D>("SubViewport/MeshInstance3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetActive(Mesh newMesh)
	{
		itemMeshPreview.Mesh = newMesh;

	}

	public void Deactivate()
	{
		itemMeshPreview.Mesh = null;
	}
}
