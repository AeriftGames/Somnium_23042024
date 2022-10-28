extends Control

var rotating: bool = false
var prev_mouse_position
var next_mouse_position

var isNowInteract = true
var passed_object
var node_interact
var player


func _ready():
	self.hide()
	$Control.hide()
	#$SubViewport.transparent_bg = true


func _process(delta):
	var texture = $SubViewport.get_texture()
	$Sprite2D.texture = texture
	_used(delta)


func show_inspect(state):
	if state:
		self.show()
	else:
		self.hide()


func _used(delta):
	if (Input.is_action_just_pressed("Rotate")):
		rotating = true
		prev_mouse_position = get_viewport().get_mouse_position()
	if (Input.is_action_just_released("Rotate")):
		rotating = false
	if rotating == true:
		next_mouse_position = get_viewport().get_mouse_position()
		$SubViewport.get_child(0).get_child(0).rotate_y((next_mouse_position.x - prev_mouse_position.x) * 1 * delta)
		$SubViewport.get_child(0).get_child(0).rotate_z(-(next_mouse_position.y - prev_mouse_position.y) * 1 * delta)
		prev_mouse_position = next_mouse_position
