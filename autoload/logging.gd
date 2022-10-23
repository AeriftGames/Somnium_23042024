extends Node

var debug_level: String
#var debug_levels : Array = ["INFO", "WARNING", "ERROR"]
var autoload_complete: bool = false


func _ready():
	Logging.info(self, "Debugger loaded")
	

func info(node: Object, text: String):
	var msg: String
	if Settings.logging_level == "INFO" or autoload_complete == false:
		if Settings.logging_include_instances == true:
			msg = "INFO: %s (%s): %s" % [node.name, node, text]
		else:
			msg = "INFO: %s: %s" % [node.name, text]
		print(msg)
	
	
func warning(node: Object, text: String):
	var msg: String
	if Settings.logging_level == "WARNING" or autoload_complete == false:
		if Settings.logging_include_instances == true:
			msg = "WARNING: %s (%s): %s" % [node.name, node, text]
		else:
			msg = "WARNING: %s: %s" % [node.name, text]
		print(msg)
	
	
func error(node: Object, text: String):
	var msg: String
	if Settings.logging_level == "ERROR" or autoload_complete == false:
		if Settings.logging_include_instances == true:
			msg = "ERROR: %s (%s): %s" % [node.name, node, text]
		else:
			msg = "ERROR: %s: %s" % [node.name, text]
		print(msg)
