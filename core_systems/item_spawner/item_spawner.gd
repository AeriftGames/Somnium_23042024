extends Node

var spawn_point
var spawn_points
var spawn_density = 0.2
var battery_scene = load("res://objects/battery/battery.tscn")
var random = RandomNumberGenerator.new()


# Called when the node enters the scene tree for the first time.
func _ready():
	random.randomize()
	spawn_point = self.get_node("/root/worldlevel/Items/spawn_points")
	_choose()
	
	#for point in spawn_points:
		#_spawn(point)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
	

func _choose():
	var count: int = spawn_point.get_child_count()
	var items_to_spawn: int =  count * spawn_density
	items_to_spawn = floor(items_to_spawn)
	var random_point = randf_range(0, count)
	spawn_points = spawn_point.get_children()
	for x in items_to_spawn:
		_spawn(spawn_points[random_point])
		spawn_points = spawn_point.get_children()
		count = spawn_point.get_child_count()
		random_point = randf_range(0, count)


func _spawn(point):
	var new = battery_scene.instantiate()
	new.global_position = point.global_position
	new.scale = Vector3(0.1, 0.1, 0.1)
	self.add_child(new)
	point.queue_free()
