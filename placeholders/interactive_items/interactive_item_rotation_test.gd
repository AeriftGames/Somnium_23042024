extends RigidBody3D

var rotating: bool = false
var prev_mouse_position
var next_mouse_position

var isNowInteract = false
var passed_object
var node_interact

# Called when the node enters the scene tree for the first time.
func _ready():
	node_interact = $interactive_object
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
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
		print("rotating")
		next_mouse_position = get_viewport().get_mouse_position()
		print(next_mouse_position)
		$MeshInstance3D.rotate_y((next_mouse_position.x - prev_mouse_position.x) * 1 * delta)
		$MeshInstance3D.rotate_z(-(next_mouse_position.y - prev_mouse_position.y) * 1 * delta)
		prev_mouse_position = next_mouse_position

func _used_quit():
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
