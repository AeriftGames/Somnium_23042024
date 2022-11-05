class_name custom_settings extends Node
@icon("res://autoload/custom_settings_icon.svg")

## Used for loading settings.cfg file.
##
## Universal loader for script.ini located in [b]"res://autoload/settings.cfg"
##[/b]. It is included in Autoloader so it can be accessed any time

# Script variables
## Used for loading config file
var cfg: ConfigFile = ConfigFile.new()
## Used for checking errors when loading Config File
var e = cfg.load("res://autoload/settings.cfg")

# Config variables
## Checked whenever Oalar wants to log something
var debug_oalar: bool
## Checked whenever Oalar wants to log something
var debug_kaen: bool
## Logging level used in [logging]. Levels are: INFO, WARNING, ERROR.
var logging_level: String
## Includes unique node names in logs.
var logging_include_instances: bool
## Logging to file log.
var logging_file_log: bool
## Clearing log file before each run.
var logging_clear_file: bool
## Include datetime with each log
var logging_include_datetime: bool

var GameMaster: Node


func _ready() -> void:
	Logging.info(self, "Settings loaded")
	_load()
	GameMaster = get_tree().root.get_node("GameMaster")


func _load() -> void:
	if e != OK:
		var message_failed: String = "Failed to load settings.cfg file. "
		var reason: String
		var msg: String
		if e == 7:
			reason = "Not found"
		elif e == 10:
			reason = "No permission"
		elif e == 11:
			reason = "Already in use"
		elif e == 14:
			reason = "Cannot read"
		elif e == 16:
			reason = "Corrupt error"
		elif e == 43:
			reason = "Parse error"
		else:
			reason = "Uknnown error"
		msg = message_failed + reason
		Logging.error(self, msg)
		return
	else:
		Logging.info(self, "Successfully loaded settings.cfg")
		_get_values()


func _get_values() -> void:
	debug_oalar = cfg.get_value("debug", "debug_oalar")
	debug_kaen = cfg.get_value("debug", "debug_kaen")
	logging_level = cfg.get_value("logging", "logging_level")
	logging_include_instances = cfg.get_value("logging", "include_instances")
	logging_file_log = cfg.get_value("logging", "file_log")
	logging_clear_file = cfg.get_value("logging", "clear_file")
	logging_include_datetime = cfg.get_value("logging", "include_datetime")

func message_update() -> void:
	var msg = GameMaster.msgObject.GetMessage()
	if(msg == "msg_get_kaen_debug_bool"):
		GameMaster.msgObject.SetBoolData(debug_kaen)
