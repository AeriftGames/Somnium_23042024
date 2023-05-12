using Godot;
using Godot.Collections;
using System;

public partial class BasicButtonObject : Node3D
{
    // objekt s kterym komunikujeme
    interactive_object inter_object;
    [Export] public string ObjectName = "item";
    [Export] public string UseActionText = "use";

    [Export] public Array<NodePath> AllUseObjectsNodePaths;

    public Array<Node> AllUseObjects = null;

    public override void _Ready()
	{
        inter_object = GetNode<interactive_object>("interactive_object");

        // Add all use objects for this button
        AllUseObjects = new Array<Node>();
        foreach (var useObjectPath in AllUseObjectsNodePaths)
        {
            if(!useObjectPath.IsEmpty)
                AllUseObjects.Add(GetNode(useObjectPath));
        }
    }

    public void UseAction(FPSCharacter_Interaction player)
    {
        GD.Print("basic button use by: " + player.Name);
        UseButton();
    }

    public void message_update()
    {
        string msg = inter_object.msgObject.GetMessage();
        switch (msg)
        {
            case "msg_use_action":
                {
                    UseAction((FPSCharacter_Interaction)inter_object.msgObject.GetNodeData());
                    break;
                }
            case "msg_get_use_action_text":
                {
                    inter_object.msgObject.SetStringData(UseActionText);
                    break;
                }
            case "msg_get_interactive_object_name":
                {
                    inter_object.msgObject.SetStringData(ObjectName);
                    break;
                }
            default: break;
        }
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
