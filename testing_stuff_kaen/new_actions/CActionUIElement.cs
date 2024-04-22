using Godot;
using System;

public partial class CActionUIElement : PanelContainer
{
    private Label ActionNameLabel;
    private Label ActionInputLabel;

    public void StartInit()
    {
        ActionNameLabel = GetNode<Label>("%ActionNameLabel");
        ActionInputLabel = GetNode<Label>("%ActionInputLabel");
    }

    public void SetData(string newActionName,CBaseAction.EActionInputEnum newActionInput)
    {
        ActionNameLabel.Text = newActionName;
        ActionInputLabel.Text = newActionInput.ToString();
    }
}
