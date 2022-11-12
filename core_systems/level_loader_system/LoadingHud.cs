using Godot;
using System;

// TODO dodelat pridavny showPressAnyKeyToStart
public partial class LoadingHud : Control
{
	Label nameOfLevelLabel = null;
	Label shaderProcessText = null;

	public override void _Ready()
	{
		nameOfLevelLabel = GetNode<Label>("NameOfLevelLabel");
		shaderProcessText = GetNode<Label>("ShaderPrecompLabelVBox/ShaderProcessText");
	}

	public void SetInitializeAndVisibleNow(string newNameOfLevel, bool newShowShaderProcess)
	{
		nameOfLevelLabel.Text = newNameOfLevel;
	}

	public void LoadingIsComplete(bool newShowPressAnyKeyToStart)
	{
		QueueFree();
	}

	public void SetShaderProcessValueText(string newShaderProcessValueText)
	{
		shaderProcessText.Text = "PRECOMPLILE SHADER " + newShaderProcessValueText + "%";
	}
}
