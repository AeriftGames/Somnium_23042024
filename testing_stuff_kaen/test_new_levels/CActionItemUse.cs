using Godot;
using System;

public partial class CActionItemUse : Node3D
{
	FPSCharacterAction ActionCharacter;
	[Export] FocusActionObject FocusActionObject;

    public override void _Ready()
	{
		GameStart();
    }

	public override void _Process(double delta)
	{
    }

	public async void GameStart()
	{
		await ToSignal(CGameMaster.GM.GetMasterSignals(), CMasterSignals.SignalName.GameStart);

        ActionCharacter = CGameMaster.GM.GetGame().GetFPSCharacterBase() as FPSCharacterAction;

		if(ActionCharacter != null)
			ActionCharacter.GetFocusActionComponent().FocusActionActive += CActionItemUse_FocusActionActive;
    }

    private void CActionItemUse_FocusActionActive(bool newResult)
    {
        //GetNode<Control>("Control").Visible = newResult;
        GetNode<Sprite3D>("Sprite3D").Visible = newResult;
    }

    public virtual void UseAction()
	{
		if(ActionCharacter == null) return;

		ActionCharacter.GetFocusActionComponent().StartFocusAction(FocusActionObject.GetFocusMovePoint(),
			FocusActionObject.GetFocusLookPoint(),1.0f,1.0f);
	}

	public virtual void HoverAction(bool newHover) 
	{
		GD.Print("Hover: " + newHover);
	}

	public void _on_button_pressed()
	{
        if (ActionCharacter == null) return;

		ActionCharacter.GetFocusActionComponent().EndFocusAction();
    }
}
