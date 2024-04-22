using Godot;
using Godot.Collections;
using System;

public partial class BasicButtonObject : Node3D
{
    [Export] public string ObjectName = "item";
    [Export] public string UseActionText = "use";


    [Export] public Array<Node> AllUseObjects = null;

    public override void _Ready()
	{
    }

    public void UseAction()
    {
        GD.Print("basic button use by player");
        UseButton();
    }

    public void UseButton()
    {
        foreach (var useObject in AllUseObjects)
        {
            useObject.Call("UseActionByButton");
        }

        UseButtonEffect();
    }

    public virtual void UseButtonEffect()
    {
        // zavola se pri pouziti
    }
}
