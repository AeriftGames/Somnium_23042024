using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class GameMaster : Node
{
	// CORE
	public static GameMaster GM;
	public MessageObject msgObject;

	// LOG
	public LogSystem Log;
	public Node GDNode_CustomSettings;
	public Node GDNode_Logging;

	// LEVEL LOADER
	public CLevelLoader LevelLoader = null;
	public global_settings Settings = null;

	// POINTERS
	private DebugHud _debugHud = null;
	private LoadingHud loadingHud = null;
	private FPSCharacter_BasicMoving _fpsCharacter = null;

	//
	private Control blackScreen = null;

	// pro testovani hledani objektu podle id
	[Export] ulong needObjectID = 1;

	//
	private bool isQuitting = false;

    public override void _Ready()
	{
		GD.Print("GameMaster loaded");
		GM = this;

		blackScreen = GetNode<Control>("BlackScreen");

		// zapnout cernou obrazovku
		EnableBlackScreen(true);

		// nacteme si objekty z autoloadu pro pristup do jejich gdscriptu
		GDNode_CustomSettings = GetTree().Root.GetNode<Node>("CustomSettings");
		GDNode_Logging = GetTree().Root.GetNode<Node>("Logging");

		// msgObject gamemastera se zapnutym multicommunication
		msgObject = new MessageObject(this, GDNode_CustomSettings, true);

		// vytvoreni csharp logging systemu
		Log = new LogSystem(this);

		// vytvoreni LevelLoaderu, druhy parametr = pouziti ShadersPrecompilation?
		LevelLoader = new CLevelLoader(this, true);

		// vytvoreni Settings - global_settings
		Settings = new global_settings(this);
    }

	// Set/Get FPS Character
	public void SetFPSCharacter(FPSCharacter_BasicMoving newFpsCharater) { _fpsCharacter = newFpsCharater; }
	public FPSCharacter_BasicMoving GetFPSCharacter() { return _fpsCharacter; }

	// Set/Get Debug Hud
	public void SetDebugHud(DebugHud newDebugHud) { _debugHud = newDebugHud; }
	public DebugHud GetDebugHud() { return _debugHud; }

	// Set/Get Loading Hud
	public void SetLoadingHud(LoadingHud newLoadingHud) { loadingHud = newLoadingHud; }
	public LoadingHud GetLoadingHud() { return loadingHud; }

	// prekryje veskery hud a 3d svet cernou obrazovkou
	public void EnableBlackScreen(bool newEnable){ blackScreen.Visible = newEnable; }

    public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				QuitGame();
		}
    }

	public async void QuitGame()
	{
		Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"Quiting Game");

		isQuitting = true;

		await TaskQuitGame();
	}

    async Task TaskQuitGame()
    {
		// Unload level process
        LevelLoader.UnloadLevelProcess();

        await Task.Delay(1000);

        // zapneme cernou obrazovku
        EnableBlackScreen(true);

        Node level = GetNode("/root/worldlevel");
		var a = level.FindChildren("*", "RigidBody3D", true, true);
		/*
		GD.Print(a.Count);
		foreach (var item in a)
		{
			GD.Print(item.Name);
		}
		*/
		msgObject.FreeAll();
		LevelLoader.Free();
        Settings.Free();
        Log.Free();

		SafeQueueAll();
		GetTree().Quit();
    }

    public void SafeQueueAll()
	{
		// Vypne _Process GameMastera
		ProcessMode = ProcessModeEnum.Disabled;

		// uvolni kameru a celeho hrace
		_fpsCharacter.objectCamera.FreeAll();
        _fpsCharacter.objectCamera.QueueFree();
        _fpsCharacter.FreeAll();
		_fpsCharacter.QueueFree();

		// pokud byl hrac uspesne zavolan pro delete, vypiseme to v konzoli
		if (_fpsCharacter.IsQueuedForDeletion())
            GD.Print("Game is quit sucesfully");
	}

	public bool GetIsQuitting()
	{
		return isQuitting;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("testDebug"))
			GD.Print("objekt ktery hledame ma nazev: " + GD.InstanceFromId(needObjectID).GetClass());

        LevelLoader.Update(delta);
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