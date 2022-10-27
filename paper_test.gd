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
	if isNowInteract == true:
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


func _used_quit():
	player.show_inspect(false)
	passed_object.SetInputEnable(true)
	passed_object = null
	isNowInteract = false


func _input(event):
	if event.is_action_pressed("Jump") and isNowInteract == true:
		_used_quit()


func message_update():
	var msg:String = node_interact.msgObject.GetMessage()
	if msg == "msg_get_use_action_text":
		node_interact.msgObject.SetStringData("Rotate item")
	if msg == "msg_get_interactive_object_name":
		node_interact.msgObject.SetStringData("placeholder")
	if msg == "msg_use_action":
		passed_object = node_interact.msgObject.GetNodeData()
		passed_object.SetInputEnable(false)
		isNowInteract = true
		player.show_inspect(true)
