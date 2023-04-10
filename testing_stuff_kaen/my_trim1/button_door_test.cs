using Godot;
using Godot.Collections;
using System;
using System.Security.AccessControl;

public partial class button_door_test : Node3D
{
    // objekt s kterym komunikujeme
    interactive_object inter_object;
    [Export] public string ObjectName = "item";
    [Export] public string UseActionText = "use";

    MeshInstance3D meshButton = null;
	Vector3 originalPos;

    [Export] bool pressed = false;

    [Export] public Array<NodePath> doors;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        inter_object = GetNode<interactive_object>("interactive_object");

        meshButton = GetNode<MeshInstance3D>("button");
        originalPos = meshButton.Position;
		SetInstantPressed(pressed);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetInstantPressed(bool newPressed)
	{
		if (meshButton == null) return;

		if(newPressed)
		{
			//pressed
			meshButton.Position = originalPos + new Vector3(0, -0.015f, 0);
            pressed = true;
		}
		else
		{
			//unpressed
			meshButton.Position = originalPos;
            pressed = false;
        }
	}

    public void UseAction(FPSCharacter_Interaction player)
    {
        GD.Print("door button use by: " + player.Name);
        SetInstantPressed(!pressed);
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
