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

	// POINTERS
	private DebugHud _debugHud = null;
	private LoadingHud loadingHud = null;
	private FPSCharacter_BasicMoving _fpsCharacter = null;

	//
	private Control blackScreen = null;

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

		// vytvoreni LevelLoaderu
		LevelLoader = new CLevelLoader(this, true);
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
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				QuitGame();
    }

	public async void QuitGame()
	{
		Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"Quiting Game");
		await TaskQuitGame();
	}

    async Task TaskQuitGame()
    {
		// Unload level process
        LevelLoader.UnloadLevelProcess();

        await Task.Delay(1000);

        // zapneme cernou obrazovku
        EnableBlackScreen(true);

		SafeQueueAll();
		GetTree().Quit();
    }

    public void SafeQueueAll()
	{
        _fpsCharacter.objectCamera.QueueFree();
        _fpsCharacter.FreeAll();
		_fpsCharacter.QueueFree();

		if (_fpsCharacter.IsQueuedForDeletion())
			GD.Print("fps character queued done");
	}

    public override void _Process(double delta)
	{
		// Update LOADING
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