extends Node
class_name logging

## Used as a Autoload for logging purposes
##
## Autoloaded script used for logging. It can create both 
## [method @GlobalScope.Print] logs and logs on FileSystem. Configuration for
## this script is taken from [custom_settings] and settings.gd. For best resultst
## his script should always be on the first place in Autoload order.

## Because of potentional need for some Autoload modules to use this logging
## script it is using autoload_complete property. This property get's changed
## to true after last Autoload script loads.
var autoload_complete: bool = false


func _ready():
	_clear_log()
	_initial_log()
	Logging.info(self, "Debugger loaded")

## Used for clearing the log file. Used only when logging_clear_file = true in
## [custom_settings]
func _clear_log():
	if CustomSettings.logging_clear_file == true or autoload_complete == false:
		var filew = FileAccess.open("res://log/log.txt", FileAccess.WRITE)
		filew.store_string("")

## Used for calling creation of the INFO log.
func info(node: Object, text: String):
	if CustomSettings.logging_level == "INFO" or autoload_complete == false:
		_create_msg("INFO", node, text)
	
## Used for calling creation of the WARNING log.
func warning(node: Object, text: String):
	if CustomSettings.logging_level == "WARNING" or autoload_complete == false:
		_create_msg("WARNING", node, text)
	
## Used for calling creation of the ERROR log.
func error(node: Object, text: String):
	if CustomSettings.logging_level == "ERROR" or autoload_complete == false:
		_create_msg("ERROR", node, text)


func _create_msg(level: String, node: Object, text: String):
	var msg: String
	var datetime: String = str(Time.get_datetime_string_from_system())
	if CustomSettings.logging_include_instances == true or autoload_complete == false:
		if CustomSettings.logging_include_datetime == true or autoload_complete == false:
			msg = "%s %s: %s (%s): %s" % [level, datetime, node.name, node, text]
		msg = "%s: %s (%s): %s" % [level, node.name, node, text]
	else: # Needs imrpoving TODO
		if CustomSettings.logging_include_datetime == true or autoload_complete == false:
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
	if CustomSettings.logging_file_log == true or autoload_complete == false:
		var filer = FileAccess.open("res://log/log.txt", FileAccess.READ)
		var content = filer.get_as_text()
		content += msg + "\r"
		var filew = FileAccess.open("res://log/log.txt", FileAccess.WRITE)
		filew.store_string(content)
