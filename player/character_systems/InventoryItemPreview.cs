using Godot;
using System;

public partial class InventoryItemPreview : SubViewportContainer
{
	MeshInstance3D itemPreviewMesh = null;

	bool isRotation = false;
	float speedRotation = 1f;

	public override void _Ready()
	{
		itemPreviewMesh = GetNode<MeshInstance3D>("SubViewport/MeshInstance3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (itemPreviewMesh == null) return;
		if (itemPreviewMesh.Mesh == null) return;
		if (!isRotation) return;

		itemPreviewMesh.Rotate(Vector3.Up, speedRotation*(float)delta);
	}

	public void Activate(Mesh newMesh,bool newRotation = false)
	{
		itemPreviewMesh.Mesh = newMesh;
		Visible = true;

		if(newRotation)
			isRotation = true;
	}

	public void Deactivate() 
	{
		Visible = false;
        isRotation = false;
        itemPreviewMesh.Mesh = null;
	}
}
