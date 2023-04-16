extends Camera3D

var order: int = 0
var current_target: Vector3
var previous_target: Vector3 
var benchmark_nodes: Node


# Called when the node enters the scene tree for the first time.
func _ready():
	benchmark_nodes = get_parent().get_node("Level/Benchmark")
	previous_target = benchmark_nodes.get_child(order).global_position
	current_target = benchmark_nodes.get_child(order).global_position
	_move_camera(current_target)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	_rotate(delta)
	#print(delta)
	pass

func _move_camera(point):
	var tween = create_tween()
	tween.tween_property(self, "global_position", point, 7)
	await tween.finished
	print("FINISHED")
	order += 1
	previous_target = current_target
	current_target = benchmark_nodes.get_child(order).global_position
	_move_camera(current_target)
	
func _rotate(delta):
	var x = lerp(previous_target, current_target, delta * 1)
	look_at(x, Vector3.UP)
