extends Camera3D

var order: int = 0
var current_target: Node
var previous_target: Node 
var benchmark_nodes: Node


# Called when the node enters the scene tree for the first time.
func _ready():
	benchmark_nodes = get_parent().get_node("Level/Benchmark")
	previous_target = benchmark_nodes.get_child(order)
	current_target = benchmark_nodes.get_child(order)
	_move_camera(current_target)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#_rotate(delta)
	#print(delta)
	rotate_towards(current_target, 1, delta)
	pass


func _move_camera(point):
	print("STARTING MOVE CAMERA")
	var tween = create_tween()
	tween.tween_property(self, "global_position", point.global_position, 7)
	await tween.finished
	print("FINISHED")
	order += 1
	previous_target = current_target
	current_target = benchmark_nodes.get_child(order)
	_move_camera(current_target)


func _rotate(delta):
	var x = lerp(previous_target, current_target, delta * 1)
	look_at(x, Vector3.UP)


func rotate_towards(target: Node, speed: float, delta):
	var direction = (target.global_transform.origin - global_transform.origin).normalized()
	var angle = direction.angle_to(Vector3.UP)
	if direction.y < 0:
		angle = -angle
	var rotation_speed = min(speed * delta, abs(angle))
	var rotation_axis = direction.cross(Vector3.UP)
	rotation *= Quaternion(rotation_axis, rotation_speed * sign(angle))
	
