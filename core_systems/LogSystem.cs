using Godot;
using System;

public partial class LogSystem : Godot.Object
{
	GameMaster gm;
	public enum ELogMsgType{INFO,WARNING,ERROR}

	public LogSystem(Node ownerInstance)
	{
		gm = (GameMaster)ownerInstance;
	}

	public bool GetIsDebugKaen()
	{
        gm.msgObject.SendMessageToGDNow_ToObject("msg_get_kaen_debug_bool", gm.GDNode_CustomSettings);
		return gm.msgObject.GetBoolData();
    }

	// Hlavni public funkce pro napsani logu
    public void WriteLog(Node newSenderNode,ELogMsgType newLogMessageType,string newText)
	{
		// pokud jsme ve fazi ukoncit hru, vyskocime pryc
		if (gm.GetIsQuitting()) return;

		// !!!!!!!! Pokud je debug kaen vypnuty, nepokracujeme dal a vyskocime z funkce !!!!!!!!!
		if (GetIsDebugKaen() == false) return;

		// nastavime parametr newSenderNode jako NodeData
		gm.msgObject.SetNodeData(newSenderNode);

		// nastavime parametr newLogMessageType jako IntData
		switch (newLogMessageType)
		{
			case ELogMsgType.INFO:
				{
                    gm.msgObject.SetIntData(0);
                    break;
                }
			case ELogMsgType.WARNING:
				{
                    gm.msgObject.SetIntData(1);
                    break;
                }
			case ELogMsgType.ERROR:
				{
                    gm.msgObject.SetIntData(2);
                    break;
                }
            default:
				break;
		}

		// nastavime parametr newText jako StringData
		gm.msgObject.SetStringData(newText);

		// posleme zpravu gd objektu Logging
		gm.msgObject.SendMessageToGDNow_ToObject("msg_new_logging_from_csharp",gm.GDNode_Logging);
	}
}