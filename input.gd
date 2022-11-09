extends HBoxContainer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$LineEdit.grab_focus()
	$Label.text = get_parent().get_parent().get_parent().get_parent().default_text


func _on_line_edit_text_submitted(new_text: String) -> void:
	get_parent().get_parent().get_parent().get_parent().submit(new_text)
	#self.queue_free()
