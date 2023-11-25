using Godot;
using System;

public partial class all_this_shaders_need_compiled : Node3D
{
    // if false = after shader compile = all visible to false only
    // if true = after shader compile = all visible to false and next timer(visible) cycle = queu free
    [Export] public bool needAllQueueFreeAfter = true;

    [Export] public float TimerVisibleInSeconds = 1.25f;
    [Export] public float TimerToggleInSeconds = 0.5f;

    Node3D AllInteractiveItemsNeedToggle;

    // delay timer
    Godot.Timer visible_timer = null;
    Godot.Timer toggle_timer = null;

    bool isAllUnvisible = false;
    bool wasFirstToggle = false;

	public override void _Ready()
	{
        visible_timer = new Godot.Timer();
        toggle_timer = new Godot.Timer();

        AllInteractiveItemsNeedToggle = GetNode<Node3D>("AllInteractiveItemsNeedToggle");

		Visible = true;

        // Create timer for visible and queuefree
        var callable_func_visible = new Callable(this,"DoneVisible");
        visible_timer.Connect("timeout", callable_func_visible);
        visible_timer.WaitTime = TimerVisibleInSeconds;
        visible_timer.OneShot = true;
        AddChild(visible_timer);
        visible_timer.Start();

        // Create timer for toggle enable and toggle disable
        var callable_func_toggle = new Callable(this,"ToggleAllInteractiveItems");
        toggle_timer.Connect("timeout", callable_func_toggle);
        toggle_timer.WaitTime = TimerToggleInSeconds;
        toggle_timer.OneShot = true;
        AddChild(toggle_timer);
        toggle_timer.Start();

        // Prepne baterku na enable
        FPSCharacter_Interaction a = (FPSCharacter_Interaction)CGameMaster.GM.GetGame().GetFPSCharacter();
        a.ToggleSimpleFlashlight();
    }

    public void DoneVisible()
    {
        if(!isAllUnvisible)
        {
            // first timer(visible) cycle
            CGameMaster.GM.GetGame().GetLevelLoader().SetNewInfoLevelCompilingShader("PRECOMPLILE SHADER PROCESS - ALL SCENES CALL VISIBLE TO FALSE", 75);
            Visible = false;
            isAllUnvisible = true;
            visible_timer.Start();
        }
        else
        {
            // Prepne baterku na disable
            FPSCharacter_Interaction a = (FPSCharacter_Interaction)CGameMaster.GM.GetGame().GetFPSCharacter();
            a.ToggleSimpleFlashlight();

            // second timer(visible) cycle
            CGameMaster.GM.GetGame().GetLevelLoader().SetNewInfoLevelCompilingShader("PRECOMPLILE SHADER PROCESS - ALL SCENES CALL QUEUEFREE" , 100);
            CGameMaster.GM.GetGame().GetLevelLoader().EndPrecompileShaderProcess();

            //
            visible_timer.Stop();
            visible_timer.QueueFree();
            toggle_timer.Stop();
            toggle_timer.QueueFree();

            QueueFree();
        }
    }

    public void ToggleAllInteractiveItems()
    {
        var a = AllInteractiveItemsNeedToggle.GetChildren();

        if(!wasFirstToggle)
        {
            // first timer(toggle) cycle
            CGameMaster.GM.GetGame().GetLevelLoader().SetNewInfoLevelCompilingShader("PRECOMPLILE SHADER PROCESS - ALL ITEMS(TOGGLED) TOGGLE FIRST", 25);
            foreach (var item in a)
            {
                item.Call("ToggleEnable");
            }
            wasFirstToggle = true;
            toggle_timer.Start();
        }
        else
        {
            // second timer(toggle) cycle
            CGameMaster.GM.GetGame().GetLevelLoader().SetNewInfoLevelCompilingShader("PRECOMPLILE SHADER PROCESS - ALL ITEMS(TOGGLED) TOGGLE SECOND", 50);
            foreach (var item in a)
            {
                item.Call("ToggleEnable");
            }
        }
    }


}
