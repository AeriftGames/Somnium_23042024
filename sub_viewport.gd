extends SubViewport


var rotating: bool = false
var prev_mouse_position
var next_mouse_position

var isNowInteract = false
var passed_object
var node_interact
var player


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func _process(delta):
	_used(delta)

	
func _used(delta):
	if (Input.is_action_just_pressed("Rotate")):
		rotating = true
		prev_mouse_position = get_viewport().get_mouse_position()
	if (Input.is_action_just_released("Rotate")):
		rotating = false
	if rotating == true:
		next_mouse_position = get_viewport().get_mouse_position()
		$PaperTest.rotate_y((next_mouse_position.x - prev_mouse_position.x) * 1 * delta)
		$PaperTest.rotate_z(-(next_mouse_position.y - prev_mouse_position.y) * 1 * delta)
		prev_mouse_position = next_mouse_position
