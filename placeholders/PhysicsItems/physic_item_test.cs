using Godot;
using System;

public partial class physic_item_test : RigidBody3D
{
    [Export] string UseActionText = "no action";
    [Export] string ObjectName = "box";

    interactive_object interactiveObject;

	public override void _Ready()
	{
        interactiveObject = GetNode<interactive_object>("interactive_object");
	}

	public override void _Process(double delta)
	{
	}

    public void UseAction(FPSCharacter_Interaction character)
    {
    }

    public void ApplyGrab(bool newGrab,FPSCharacter_Interaction character)
    {

    }

    public void message_update()
    {
        string msg = interactiveObject.msgObject.GetMessage();
        switch (msg)
        {
            case "msg_get_use_action_text":
                {
                    interactiveObject.msgObject.SetStringData(UseActionText);
                    break;
                }
            case "msg_get_interactive_object_name":
                {
                    interactiveObject.msgObject.SetStringData(ObjectName);
                    break;
                }
            case "msg_use_action":
                {
                    UseAction((FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData());
                    break;
                }
            case "msg_apply_grab":
                {
                    ApplyGrab(interactiveObject.msgObject.GetBoolData(),
                        (FPSCharacter_Interaction)interactiveObject.msgObject.GetNodeData());
                    break;
                }
            default: break;
        }
    }
}
