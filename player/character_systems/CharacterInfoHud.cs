using Godot;
using System;

public partial class CharacterInfoHud : Control
{
    ProgressBar progressBarHealth = null;
    Label labelHealthVal = null;

    public override void _Ready()
    {
        base._Ready();

        progressBarHealth = GetNode<ProgressBar>("HBoxContainer_Health/ProgressBar_Health");
        labelHealthVal = GetNode<Label>("HBoxContainer_Health/ProgressBar_Health/Label_HealthValue");

        SetDataFromPlayer();
    }

    public void _on_progress_bar_health_value_changed(float val)
    {
        labelHealthVal.Text = val.ToString();
    }

    public void SetDataFromPlayer()
    {
        FPSCharacter_Inventory character = GameMaster.GM.GetFPSCharacter() as FPSCharacter_Inventory;
        if (character == null) return;

        progressBarHealth.Value = character.GetHealthSystem().GetHealth();
        progressBarHealth.MaxValue = character.GetHealthSystem().GetMaxHealth();
        progressBarHealth.MinValue = 0.0f;
    }
}
