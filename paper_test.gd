extends MeshInstance3D

var rotating: bool = false
var prev_mouse_position
var next_mouse_position

var isNowInteract = true
var passed_object
var node_interact
var player


func _ready():
	player = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect")
	node_interact = $interactive_object


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
		rotate_y((next_mouse_position.x - prev_mouse_position.x) * 1 * delta)
		rotate_z(-(next_mouse_position.y - prev_mouse_position.y) * 1 * delta)
		prev_mouse_position = next_mouse_position
