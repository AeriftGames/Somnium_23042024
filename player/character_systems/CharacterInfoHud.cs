using Godot;
using System;

public partial class CharacterInfoHud : Control
{
    ProgressBar progressBarHealth = null;
    Label labelHealthVal = null;

    ProgressBar progressBarStamina = null;
    Label labelStaminaVal = null;

    public override void _Ready()
    {
        base._Ready();

        progressBarHealth = GetNode<ProgressBar>("VBoxContainer/HBoxContainer_Health/ProgressBar_Health");
        labelHealthVal = GetNode<Label>("VBoxContainer/HBoxContainer_Health/ProgressBar_Health/Label_HealthValue");
        SetHealthDataFromPlayer();

        progressBarStamina = GetNode<ProgressBar>("VBoxContainer/HBoxContainer_Stamina/ProgressBar_Stamina");
        labelStaminaVal = GetNode<Label>("VBoxContainer/HBoxContainer_Stamina/ProgressBar_Stamina/Label_StaminaValue");
        SetStaminaDataFromPlayer();
    }

    public void _on_progress_bar_health_value_changed(float val)
    {
        labelHealthVal.Text = val.ToString();
    }

    public void _on_progress_bar_stamina_value_changed(float val)
    {
        labelStaminaVal.Text = val.ToString();
    }

    public void SetHealthDataFromPlayer()
    {
        FPSCharacter_Inventory character = CGameMaster.GM.GetGame().GetFPSCharacter() as FPSCharacter_Inventory;
        if (character == null) return;

        progressBarHealth.Value = character.GetCharacterHealthComponent().GetHealth();
        progressBarHealth.MaxValue = character.GetCharacterHealthComponent().GetMaxHealth();
        progressBarHealth.MinValue = 0.0f;
    }

    public void SetStaminaDataFromPlayer()
    {
        FPSCharacter_Inventory character = CGameMaster.GM.GetGame().GetFPSCharacter() as FPSCharacter_Inventory;
        if (character == null) return;

        progressBarStamina.Value = character.GetCharacterStaminaComponent().GetStamina();
        progressBarStamina.MaxValue = character.GetCharacterStaminaComponent().GetMaxStamina();
        progressBarStamina.MinValue = 0.0f;
    }
}
