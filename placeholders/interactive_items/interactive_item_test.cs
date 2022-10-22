using Godot;
using System;

public partial class interactive_item_test : Node3D
{
	[Export] public string ObjectName = "item";
	[Export] public string UseActionText = "use";

    // objekt s kterym komunikujeme
    interactive_object inter_object;

    public override void _Ready()
	{
        inter_object = GetNode<interactive_object>("interactive_object");
    }

	public override void _Process(double delta)
	{
	}

	public void UseAction(FPSCharacter_Interaction player)
	{
		GD.Print("Use Action by: " + player.Name);
		GD.Print("Destroying this item");
		QueueFree();
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
}
