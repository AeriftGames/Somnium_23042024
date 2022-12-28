@tool
extends VBoxContainer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var button = get_node("Button")
	button.pressed.connect(_on_button_pressed)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_button_pressed() -> void:
	$Label.text = "Pressed"
	print("Pressed")
	print(get_tree().get_edited_scene_root())
	print()
