using Godot;
using Godot.Collections;
using System;

public partial class InBenchmarkMenu : Control
{
    VBoxContainer InGameMenuButtonsContainer = null;

    private int focusButtonID = 0;
    private bool active = false;

    public override void _Ready()
    {
        InGameMenuButtonsContainer = GetNode<VBoxContainer>("Control/VBoxContainer");
        SetActive(false);
    }

    public void SetActive(bool newActive)
    {
        // vnitrni zmena stavu
        active = newActive;

        // viditelnost InGameMenu
        Visible = active;

        // ostatni akce pri zmene
        if (active)
        {
            SetActiveFocusButtonID(0);
        }
        else
        {

        }
    }

    public bool GetActive() { return active; }

    // Button BackToGame - Deaktivuje toto InGameMenu a povoli opet inputs charactera
    public void _on_button_back_to_game_pressed()
    {
        SetActive(false);
    }

    public void _on_button_restart_benchmark_pressed()
    {
        GameMaster.GM.GetBenchmarkSystem().NeedBenchmarkQualityLevel = 0;
        GameMaster.GM.GetBenchmarkSystem().StartBenchmarkLevel(
            GameMaster.GM.GetLevelLoader().GetActualLevelScene().SceneFilePath,
            GameMaster.GM.GetLevelLoader().GetActualLevelName());
    }

    public void _on_button_quit_game_pressed()
    {
        //GameMaster.GM.QuitGame();
        GetTree().Quit();
    }

    public void SetActiveFocusButtonID(int newButtonID)
    {
        focusButtonID = newButtonID;

        foreach (var item in InGameMenuButtonsContainer.GetChildren())
        {
            BaseFocusedMenuButton a = (BaseFocusedMenuButton)item;
            if (a != null)
            {
                if (a.ButtonFocusID == newButtonID)
                {
                    //GD.Print("FOCUS");
                    a.GrabFocus();
                }
            }
        }
    }

    public int GetFocusButtonID()
    {
        return focusButtonID;
    }
}
