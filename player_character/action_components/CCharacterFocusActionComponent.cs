using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class CCharacterFocusActionComponent : CBaseComponent
{
    private Tween TweenFocusMove = null;
    private Tween TweenFocusLook = null;

    private Node3D HeadFocusAction = null;
    private float TransMoveTime = 0.75f;
    private float TransLookTime = 0.75f;

    private bool isFocusActive = false;
    private Node3D actualFocusNode = null;
    private Node3D actualLookNode = new Node3D();
    private bool isFocusNotActive = true;

    [Signal] public delegate void FocusActionActiveEventHandler(bool newResult);

    public override void PostInit(FpsCharacterBase newCharacterBase)
    {
        base.PostInit(newCharacterBase);

        HeadFocusAction = ourCharacterBase.GetCharacterLookComponent().GetHeadFocusAction();

        CGameMaster.GM.GetGame().GetLevelLoader().GetActualLevelScene().AddChild(actualLookNode);
        EndFocusAction();
    }

    private void TweenFocusLook_Finished()
    {
        if (isFocusActive)
        {
            isFocusNotActive = false;

            EmitSignal(SignalName.FocusActionActive, true);
            ourCharacterBase.SetMouseVisible(true);
        }
        else
        {
            isFocusNotActive = true;

            ourCharacterBase.SetCharacterInputState(FpsCharacterBase.ECharacterInputState.Normal);
            ourCharacterBase.SetMouseVisible(false);

            actualLookNode.Position = Vector3.Zero;
        }
    }

    public void StartFocusAction(Node3D newFocusMoveNode, Node3D newFocusLookNode,
        float newTransSpeedMove,float newTransSpeedLook)
    {
        ourCharacterBase.SetCharacterInputState(FpsCharacterBase.ECharacterInputState.DontMoveAndLook);

        isFocusActive = true;
        isFocusNotActive = false;

        TransMoveTime = newTransSpeedMove;
        TransLookTime = newTransSpeedLook;

        // init
        actualLookNode.GlobalPosition = ourCharacterBase.GetCharacterLookComponent().GetHeadForwardNode().GlobalPosition;

        // pos
        if (TweenFocusMove != null)
            TweenFocusMove.Kill();

        TweenFocusMove = GetTree().CreateTween();
        TweenFocusMove.TweenProperty(
            HeadFocusAction, "global_position", newFocusMoveNode.GlobalPosition, TransMoveTime).SetTrans(
            Tween.TransitionType.Cubic);

        actualFocusNode = newFocusLookNode;

        // look
        if (TweenFocusLook != null)
            TweenFocusLook.Kill();

        TweenFocusLook = GetTree().CreateTween();
        TweenFocusLook.TweenProperty(
            actualLookNode, "position", actualFocusNode.GlobalPosition, TransLookTime).SetTrans(
            Tween.TransitionType.Cubic);

        TweenFocusLook.Finished += TweenFocusLook_Finished;
    }

    public void EndFocusAction()
    {
        ourCharacterBase.SetMouseVisible(false);
        EmitSignal(SignalName.FocusActionActive, false);

        isFocusActive = false;

        // pos
        if (TweenFocusMove != null)
            TweenFocusMove.Kill();

        TweenFocusMove = GetTree().CreateTween();
        TweenFocusMove.TweenProperty(
            HeadFocusAction, "position", new Vector3(0, 0.0f, 0),TransMoveTime).SetTrans(
            Tween.TransitionType.Cubic);

        actualFocusNode = null;

        // look
        if (TweenFocusLook != null)
            TweenFocusLook.Kill();

        TweenFocusLook = GetTree().CreateTween();
        TweenFocusLook.TweenProperty(
            actualLookNode, "position", ourCharacterBase.GetCharacterLookComponent().
            GetHeadForwardNode().GlobalPosition, TransLookTime).SetTrans(
            Tween.TransitionType.Cubic);

        TweenFocusLook.Finished += TweenFocusLook_Finished;
    }

    private void UpdateLook(double delta)
    {
        if (isFocusNotActive)
        {
            /*actualLookNode.GlobalPosition = ourCharacterBase.GetCharacterLookComponent().
                GetHeadForwardNode().GlobalPosition;*/
        }

        if(!isFocusNotActive)
            HeadFocusAction.LookAt(actualLookNode.GlobalPosition, null, false);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        UpdateLook(delta);
    }

    public bool GetIsFocusActionActive() { return isFocusActive; }
}
