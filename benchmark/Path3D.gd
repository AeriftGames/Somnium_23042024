extends Path3D

var camera: Node
var pos: Node
var look: Node
var poslerp: Vector3
var looklerp: Vector3
@export var next_scene: String
@export var level_name: String

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	camera = $Camera3D
	pos = $PathFollow3D_pos
	look = $PathFollow3D_look
	poslerp = pos.global_position
	looklerp = look.global_position
	if OS.has_feature("standalone"):
		print("Running an exported build.")
	else:
		print("Running from the editor.")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	_move_point(pos, delta)
	_move_point(look, delta)
	_move_camera(camera, delta)
	

func _move_point(point, delta):
	point.progress += delta
	
	
func _move_camera(camera, delta):
	poslerp = poslerp.lerp(pos.global_position, 0.5 * delta)
	looklerp = looklerp.lerp(look.global_position, 0.5 * delta)
	camera.global_position = poslerp
	camera.look_at_from_position(camera.global_position, looklerp)
