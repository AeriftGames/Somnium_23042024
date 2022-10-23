extends Node
# This script is used for loading settings.cfg file.

# Script variables
var cfg = ConfigFile.new()
var e = cfg.load("res://autoload/settings.cfg")

# Config variables
var debug_oalar: bool
var debug_kaen: bool
var logging_level: String
var logging_include_instances: bool
var logging_file_log: bool
var logging_clear_file: bool

func _ready():
	Logging.info(self, "Settings loaded")
	_load()


func _load():
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


func _get_values():
	debug_oalar = cfg.get_value("debug", "debug_oalar")
	debug_kaen = cfg.get_value("debug", "debug_kaen")
	logging_level = cfg.get_value("logging", "logging_level")
	logging_include_instances = cfg.get_value("logging", "include_instances")
	logging_file_log = cfg.get_value("logging", "file_log")
	logging_clear_file = cfg.get_value("logging", "clear_file")
