extends HBoxContainer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$LineEdit.grab_focus()
	$Label.text = "Enter password: "


func _on_line_edit_text_submitted(new_text: String) -> void:
	get_parent().get_parent().get_parent().get_parent().submit_password(new_text)
	self.queue_free()
