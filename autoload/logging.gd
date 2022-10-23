extends Node

var debug_level: String
#var debug_levels : Array = ["INFO", "WARNING", "ERROR"]
var autoload_complete: bool = false


func _ready():
	_clear_log()
	_initial_log()
	Logging.info(self, "Debugger loaded")


func _clear_log():
	if Settings.logging_clear_file == true or autoload_complete == false:
		var filew = FileAccess.open("res://log/log.txt", FileAccess.WRITE)
		filew.store_string("")


func info(node: Object, text: String):
	if Settings.logging_level == "INFO" or autoload_complete == false:
		_create_msg("INFO", node, text)
	
	
func warning(node: Object, text: String):
	if Settings.logging_level == "WARNING" or autoload_complete == false:
		_create_msg("WARNING", node, text)
	
	
func error(node: Object, text: String):
	if Settings.logging_level == "ERROR" or autoload_complete == false:
		_create_msg("ERROR", node, text)


func _create_msg(level: String, node: Object, text: String):
	var msg: String
	var datetime: String = str(Time.get_datetime_string_from_system())
	if Settings.logging_include_instances == true or autoload_complete == false:
		if Settings.logging_include_datetime == true or autoload_complete == false:
			msg = "%s %s: %s (%s): %s" % [level, datetime, node.name, node, text]
		msg = "%s: %s (%s): %s" % [level, node.name, node, text]
	else: # Needs imrpoving TODO
		if Settings.logging_include_datetime == true or autoload_complete == false:
			msg = "%s %s: %s: %s" % [level, datetime, node.name, text]
		msg = "%s: %s: %s" % [level, node.name, text]
	_create_log(msg)
	

func _initial_log():
	var uid: String = str(OS.get_unique_id())
	var cpu_name: String = str(OS.get_processor_name())
	var cpu_count: String = str(OS.get_processor_count())
	var os: String = str(OS.get_name())
	var version: String = str(OS.get_version())
	
	_create_log("----- HARDWARE INFORMATION -----", false)
	_create_log("Running on %s - %s" % [os, version], false)
	_create_log("UID - %s" % [uid], false)
	_create_log("CPU name: %s, CPU count: %s" % [cpu_name, cpu_count], false)
	_create_log("--------------------------------", false)

func _create_log(msg: String, prnt: bool = true):
	if prnt == true:
		print(msg)
	if Settings.logging_file_log == true or autoload_complete == false:
		var filer = FileAccess.open("res://log/log.txt", FileAccess.READ)
		var content = filer.get_as_text()
		content += msg + "\r"
		var filew = FileAccess.open("res://log/log.txt", FileAccess.WRITE)
		filew.store_string(content)
