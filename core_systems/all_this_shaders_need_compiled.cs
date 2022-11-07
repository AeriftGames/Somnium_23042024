using Godot;
using System;

public partial class all_this_shaders_need_compiled : Node3D
{
    // if false = after shader compile = all visible to false only
    // if true = after shader compile = all visible to false and next timer(visible) cycle = queu free
    [Export] public bool needAllQueueFreeAfter = true;

    [Export] public float TimerVisibleInSeconds = 1.0f;
    [Export] public float TimerToggleInSeconds = 0.5f;

    Node3D AllInteractiveItemsNeedToggle;

    // delay timer
    Godot.Timer visible_timer = new Godot.Timer();
    Godot.Timer toggle_timer = new Godot.Timer();

    bool isAllUnvisible = false;
    bool wasFirstToggle = false;

	public override void _Ready()
	{
        AllInteractiveItemsNeedToggle = GetNode<Node3D>("AllInteractiveItemsNeedToggle");

		Visible = true;

        // Create timer for visible and queuefree
        var callable_func_visible = new Callable(DoneVisible);
        visible_timer.Connect("timeout", callable_func_visible);
        visible_timer.WaitTime = 1;
        visible_timer.OneShot = true;
        AddChild(visible_timer);
        visible_timer.Start();

        // Create timer for toggle enable and toggle disable
        var callable_func_toggle = new Callable(ToggleAllInteractiveItems);
        toggle_timer.Connect("timeout", callable_func_toggle);
        toggle_timer.WaitTime = 0.5f;
        toggle_timer.OneShot = true;
        AddChild(toggle_timer);
        toggle_timer.Start();
    }

    public void DoneVisible()
    {
        if(!isAllUnvisible)
        {
            // first timer(visible) cycle
            GD.Print("all preload shaders set unvisible");
            Visible = false;
            isAllUnvisible = true;
            visible_timer.Start();
        }
        else
        {
            // second timer(visible) cycle
            GD.Print("all preload shaders are queue free");
            QueueFree();
        }
    }

    public void ToggleAllInteractiveItems()
    {
        var a = AllInteractiveItemsNeedToggle.GetChildren();

        if(!wasFirstToggle)
        {
            // first timer(toggle) cycle
            GD.Print("all interactive items toggle to enabled");
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
            GD.Print("all interactive items toggle to disabled");
            foreach (var item in a)
            {
                item.Call("ToggleEnable");
            }
        }
    }


}
