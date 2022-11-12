extends Button

signal button_toggled(button, state)
var msg_text: String

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_toggled(button_pressed: bool) -> void:
	emit_signal("button_toggled", self, button_pressed)
	$AudioStreamPlayer.play()
