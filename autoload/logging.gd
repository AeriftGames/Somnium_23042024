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
	var msg: String
	if Settings.logging_level == "INFO" or autoload_complete == false:
		if Settings.logging_include_instances == true or autoload_complete == false:
			msg = "INFO: %s (%s): %s" % [node.name, node, text]
		else:
			msg = "INFO: %s: %s" % [node.name, text]
		_create_log(msg)
	
	
func warning(node: Object, text: String):
	var msg: String
	if Settings.logging_level == "WARNING" or autoload_complete == false:
		if Settings.logging_include_instances == true or autoload_complete == false:
			msg = "WARNING: %s (%s): %s" % [node.name, node, text]
		else:
			msg = "WARNING: %s: %s" % [node.name, text]
		_create_log(msg)
	
	
func error(node: Object, text: String):
	var msg: String
	if Settings.logging_level == "ERROR" or autoload_complete == false:
		if Settings.logging_include_instances == true or autoload_complete == false:
			msg = "ERROR: %s (%s): %s" % [node.name, node, text]
		else:
			msg = "ERROR: %s: %s" % [node.name, text]
		_create_log(msg)
	

func _initial_log():
	var uid: String = str(OS.get_unique_id())
	var cpu_name: String = str(OS.get_processor_name())
	var cpu_count: String = str(OS.get_processor_count())
	var os: String = str(OS.get_name())
	var version: String = str(OS.get_version())
	var datetime = Time.get_datetime_string_from_system()
	
	_create_log("----- HARDWARE INFORMATION -----")
	_create_log("Running on %s - %s" % [os, version])
	_create_log("UID - %s" % [uid])
	_create_log("CPU name: %s, CPU count: %s" % [cpu_name, cpu_count])
	_create_log("--------------------------------")

func _create_log(msg: String):
	print(msg)
	if Settings.logging_file_log == true or autoload_complete == false:
		var filer = FileAccess.open("res://log/log.txt", FileAccess.READ)
		var content = filer.get_as_text()
		content += msg + "\r"
		var filew = FileAccess.open("res://log/log.txt", FileAccess.WRITE)
		filew.store_string(content)
