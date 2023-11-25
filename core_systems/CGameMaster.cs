using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class CGameMaster : Node
{
	// EXPORTS
	[Export] public string Build;

	// CORE
	public static CGameMaster GM;
	public MessageObject msgObject;

	// OALAR ACCESS
	public Node GDNode_CustomSettings;
	public Node GDNode_Logging;

	// OBJECTS
    private CUniversal Universal = null;
    private CGame Game = null;
	private CGlobalSettings Settings = null;
	private CDebugHud DebugHud = null;
	private CBenchmarkSystem Benchmark = null;
    private CMasterSignals MasterSignals = null;

	// DATA
	private bool isQuitting = false;

	public override void _Ready()
	{
		GD.Print("GameMaster loaded");
		GM = this;

		// nacteme si objekty z autoloadu pro pristup do jejich gdscriptu
		GDNode_CustomSettings = GetTree().Root.GetNode<Node>("CustomSettings");
		GDNode_Logging = GetTree().Root.GetNode<Node>("Logging");

        // msgObject gamemastera se zapnutym multicommunication
        msgObject = new MessageObject(this, GDNode_CustomSettings, true);

        Universal = GetNode<CUniversal>("Universal");
        Universal.PostInit();

		Settings = GetNode<CGlobalSettings>("GlobalSettings");
		Settings.PostInit();

		Benchmark = GetNode<CBenchmarkSystem>("Benchmark");
		MasterSignals = GetNode<CMasterSignals>("MasterSignals");

		Game = GetNode<CGame>("Game");
		Game.PostInit();

        // zapnout cernou obrazovku
        GetUniversal().EnableBlackScreen(true, true);
    }

	public string GetBuild(){ return Build; }

	public CGame GetGame() { return Game;}
	public CUniversal GetUniversal() { return Universal;}
	public void SetDebugHud(CDebugHud newDebugHud) { DebugHud = newDebugHud; }
	public CDebugHud GetDebugHud() { return DebugHud; }
	public CGlobalSettings GetSettings(){ return Settings; }
	public CBenchmarkSystem GetBenchmark() { return Benchmark; }
	public CMasterSignals GetMasterSignals() { return MasterSignals; }

	public void QuitGame()
	{
		GetUniversal().GetMasterLog().WriteLog(this,CMasterLog.ELogMsgType.INFO,"Quiting Game");

		isQuitting = true;
		TaskQuitGame();
	}

	public async void TaskQuitGame()
	{
        // zapneme cernou obrazovku
        GetUniversal().EnableBlackScreen(true);

		await Task.Delay(1000);

		msgObject.FreeAll();
		GetGame().GetLevelLoader().Free();
		Settings.Free();
        GetUniversal().GetMasterLog().Free();

		SafeQueueAll();

        await Task.Delay(100);
        GetTree().Quit();
	}

	public void SafeQueueAll()
	{
		// Vypne _Process GameMastera
		ProcessMode = ProcessModeEnum.Disabled;

        // uvolni kameru a celeho hrace
        GetGame().GetFPSCharacter().objectCamera.FreeAll();
		GetGame().GetFPSCharacter().QueueFree();
        GetGame().GetFPSCharacter().FreeAll();
        GetGame().GetFPSCharacter().QueueFree();

		// pokud byl hrac uspesne zavolan pro delete, vypiseme to v konzoli
		if (GetGame().GetFPSCharacter().IsQueuedForDeletion())
			GD.Print("Game is quit sucesfully");
	}

	public void QueueCharacterAndCamera()
	{
		if (GetGame().GetFPSCharacter() != null)
		{
            if (GetGame().GetFPSCharacter().objectCamera != null)
            {
                GetGame().GetFPSCharacter().objectCamera.FreeAll();
                GetGame().GetFPSCharacter().objectCamera.QueueFree();
                GetGame().GetFPSCharacter().objectCamera = null;
            }

            GetGame().GetFPSCharacter().FreeAll();
            GetGame().GetFPSCharacter().QueueFree();
			GetGame().SetFPSCharacter(null);
        }
    }

	public bool GetIsQuitting()
	{
		return isQuitting;
	}

	public override void _Process(double delta)
	{
        // pro async level load
        GetGame().GetLevelLoader().Update(delta);
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
