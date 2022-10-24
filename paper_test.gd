extends MeshInstance3D

var rotating: bool = false
var prev_mouse_position
var next_mouse_position

var isNowInteract = false
var passed_object
var node_interact

var camera_pos
var camera_look

var tween

# Called when the node enters the scene tree for the first time.
func _ready():
	node_interact = $interactive_object
	camera_pos = $camera_pos
	camera_look = $camera_look
	#tween = create_tween()
	print(tween)
	#var x = self.set_scale(Vector3(1, 1, 1))
	#self.scale = Vector3(1, 1, 1)
	#print(x)
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
		rotate_y((next_mouse_position.x - prev_mouse_position.x) * 1 * delta)
		rotate_z(-(next_mouse_position.y - prev_mouse_position.y) * 1 * delta)
		prev_mouse_position = next_mouse_position

func _used2():
	tween = create_tween()
	tween.tween_property(self, "position", Vector3(-0.741, 1.322, -8.345), 1)
	tween.tween_property(self, "rotation", Vector3(90, 0, 0), 1)
	$Timer.start(3)
	#passed_object.DisableInputsAndCameraMoveLookTarget(camera_pos.get_global_position(), camera_look.get_global_position())
	#tween.tween_property(self, "scale", Vector3(1, 1, 1), 1.5)
	#tween.tween_property(self, "position", Vector3(0, 1.5, 0), 1)
	#tween.tween_property(self, "rotation", Vector3(90, 0, 0), 1)
	

func _used_quit():
	#passed_object.SetInputEnable(true)
	passed_object.EnableInputsAndCameraToNormal()
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
		#passed_object.SetInputEnable(false)
		isNowInteract = true
		self._used2()


func _on_timer_timeout():
	passed_object.DisableInputsAndCameraMoveLookTarget(camera_pos.get_global_position(), camera_look.get_global_position())
	
