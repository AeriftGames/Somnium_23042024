using Godot;
using Godot.Collections;
using System;

public partial class CStateMachine : Node
{
    [Export] CState CURRENT_STATE;
    Dictionary<string, CState> states;

    public void PostInit() 
    {
        states = new Dictionary<string, CState>();

        foreach (var child in GetChildren())
        {
            CState state = child as CState;
            if (state != null)
            {
                states.Add(state.Name, state);
                state.Connect(CState.SignalName.Transition, new Callable(this, "OnChildTransition"));
            }
            else
                GD.PushWarning("StateMachine obsahuje jiny child nez CState");
        }

        CURRENT_STATE.Enter();
    }

    public override void _Process(double delta)
    {
        CURRENT_STATE.Update((float)delta);

        CGameMaster.GM.GetGame().GetDebugPanel().GetDebugLabels().AddProperty("Current State",CURRENT_STATE.Name.ToString(),3);
    }

    public override void _PhysicsProcess(double delta)
    {
        CURRENT_STATE.PhysicsUpdate((float)delta);
    }

    public void OnChildTransition(StringName newStateName)
    {
        CState newState = null;
        states.TryGetValue(newStateName,out newState);
        if (newState != null)
        {
            if (newState != CURRENT_STATE)
            {
                CURRENT_STATE.Exit();
                newState.Enter();
                CURRENT_STATE = newState;
            }
        }
        else
            GD.PushWarning("State neexistuje");
    }

    public StringName GetCurrentStateName() { return CURRENT_STATE.Name; }
}