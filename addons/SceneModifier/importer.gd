@tool
extends VBoxContainer

var button: Node
var log: String


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button = get_node("Button")
	button.pressed.connect(_on_button_pressed)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_button_pressed() -> void:
	var rootnode = get_tree().get_edited_scene_root()
	print(rootnode)
	log += str(rootnode) + "\n"
	var x = _getchildren(rootnode)
	$Label.text = x


func _getchildren(node: Node):
	for x in node.get_children():
		if x.get_child_count() > 0:
			print(x)
			log += str(x) + "\n"
			_getchildren(x)
		else:
			print(x)
			log += str(x) + "\n"
	return log
