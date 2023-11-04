using Godot;
using System;

// TODO dodelat pridavny showPressAnyKeyToStart
public partial class LoadingHud : Control
{
	Label nameOfLevelLabel = null;
	Label shaderProcessText = null;
	ProgressBar loadingLevelProgressBar = null;

	public override void _Ready()
	{
		nameOfLevelLabel = GetNode<Label>("NameOfLevelLabel");
		shaderProcessText = GetNode<Label>("ShaderPrecompLabelVBox/ShaderProcessText");
		loadingLevelProgressBar = GetNode<ProgressBar>("LoadingLevel_ProgressBar");
	}

	public void SetInitializeAndVisibleNow(string newNameOfLevel, bool newShowShaderProcess)
	{
		nameOfLevelLabel.Text = newNameOfLevel;
		Visible = true;
	}

	public void LoadingIsComplete(bool newShowPressAnyKeyToStart)
	{
		QueueFree();
	}

	public void SetShaderProcessValueText(string newShaderProcessValueText)
	{
		shaderProcessText.Text = "PRECOMPLILE SHADER " + newShaderProcessValueText + "%";
	}

	public void UpdateProgressBar(float percentage)
	{
		loadingLevelProgressBar.Value = percentage * 100;
    }
}
