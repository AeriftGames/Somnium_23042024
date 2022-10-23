extends Node
# This script needs to be always last in the Autoload order

func _ready():
	Logging.autoload_complete = true
	self.queue_free()
