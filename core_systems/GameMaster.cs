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
	private CLevelLoader LevelLoader = null;
	private global_settings Settings = null;

	// POINTERS
	private DebugHud _debugHud = null;
	private LoadingHud loadingHud = null;
	private FPSCharacter_BasicMoving _fpsCharacter = null;
	private CBenchmarkSystem BenchmarkSystem = null;
    private MasterSignals masterSignals = null;

    //
    private Control blackScreen = null;

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
		LevelLoader = GetNode<CLevelLoader>("LevelLoader");
		LevelLoader.PostInit(false);

		loadingHud = GetNode<LoadingHud>("LoadingHud");
		loadingHud.Visible = false;

		// vytvoreni Settings - global_settings
		Settings = new global_settings(this);

		BenchmarkSystem = GetNode<CBenchmarkSystem>("BenchmarkSystem");

		masterSignals = GetNode<MasterSignals>("MasterSignals");

    }

	// Set/Get FPS Character
	public void SetFPSCharacter(FPSCharacter_BasicMoving newFpsCharater) { _fpsCharacter = newFpsCharater; }
	public FPSCharacter_BasicMoving GetFPSCharacter() { return _fpsCharacter; }

	// Set/Get Debug Hud
	public void SetDebugHud(DebugHud newDebugHud) { _debugHud = newDebugHud; }
	public DebugHud GetDebugHud() { return _debugHud; }

	// Get Loading Hud
	public LoadingHud GetLoadingHud() { return loadingHud; }

	public CLevelLoader GetLevelLoader() { return LevelLoader; }

	// prekryje veskery hud a 3d svet cernou obrazovkou
	public void EnableBlackScreen(bool newEnable){ blackScreen.Visible = newEnable; }

	//
	public global_settings GetSettings(){ return Settings; }
	public CBenchmarkSystem GetBenchmarkSystem() { return BenchmarkSystem; }
	public MasterSignals GetMasterSignals() { return masterSignals; }

	public override void _UnhandledInput(InputEvent @event)
	{
	}

	public void QuitGame()
	{
		Log.WriteLog(this,LogSystem.ELogMsgType.INFO,"Quiting Game");

		isQuitting = true;

		TaskQuitGame();
	}

	public void ToggleInGameMenu()
	{
		FPSCharacter_Interaction interactCharacter = (FPSCharacter_Interaction)GetFPSCharacter();
		if (interactCharacter == null) return;

		// toggle stav InGameMenu
		interactCharacter.GetInGameMenu().SetActive(!interactCharacter.GetInGameMenu().GetActive());
	}

	public void TaskQuitGame()
	{
		// zapneme cernou obrazovku
		EnableBlackScreen(true);

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

	public void QueueCharacterAndCamera()
	{
		if (_fpsCharacter != null)
		{
            if (_fpsCharacter.objectCamera != null)
            {
                _fpsCharacter.objectCamera.FreeAll();
                _fpsCharacter.objectCamera.QueueFree();
                _fpsCharacter.objectCamera = null;
            }

            _fpsCharacter.FreeAll();
            _fpsCharacter.QueueFree();
            _fpsCharacter = null;
        }
    }

	public bool GetIsQuitting()
	{
		return isQuitting;
	}

	public override void _Process(double delta)
	{
		// INPUTS

        if (Input.IsActionJustPressed("EscapeAction") && GetFPSCharacter() != null)
            ToggleInGameMenu();

		// pro async level load
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
