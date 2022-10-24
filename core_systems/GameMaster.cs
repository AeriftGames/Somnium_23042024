using Godot;
using System;
using System.Diagnostics;

public partial class GameMaster : Node
{
    public static GameMaster GM;
    public MessageObject msgObject;

    public LogSystem Log;

    // nodes v autoloadu se kterymi chceme komunikovat prostrednictvim msgObject
    public Node GDNode_CustomSettings;
    public Node GDNode_Logging;

    //
    public FPSCharacter_BasicMoving FpsCharacter = null;

    public override void _Ready()
    {
        GD.Print("GameMaster loaded");
        GM = this;

        // nacteme si objekty z autoloadu pro pristup do jejich gdscriptu
        GDNode_CustomSettings = GetTree().Root.GetNode<Node>("CustomSettings");
        GDNode_Logging = GetTree().Root.GetNode<Node>("Logging");

        // msgObject gamemastera se zapnutym multicommunication
        msgObject = new MessageObject(this, GDNode_CustomSettings, true);

        // vytvoreni csharp logging systemu
        Log = new LogSystem(this);

        // Test
        Log.WriteLog(this, LogSystem.ELogMsgType.INFO, "blablasfldaslfaslf");
        Log.WriteLog(this, LogSystem.ELogMsgType.WARNING, "blablasfldaslfaslf");
        Log.WriteLog(this, LogSystem.ELogMsgType.ERROR, "blablasfldaslfaslf");
    }

    public FPSCharacter_BasicMoving GetFPSCharacter()
    {
        return FpsCharacter;
    }

    public override void _Process(double delta)
    {
    }

    public void message_update()
    {
        string msg = msgObject.GetMessage();
        switch (msg)
        {
            case "":
                {
                    break;
                }
            default:
                break;
        }
    }
}
