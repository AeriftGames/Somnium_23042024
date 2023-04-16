extends Camera3D

var order: int = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	_start()
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _start():
	var tween = create_tween()
	var tween2 = create_tween()
	for c in self.get_children():
		tween.tween_property(self, "global_position", c.global_position, 7)
		self.look_at(c.global_position, Vector3.FORWARD)
		#tween2.tween_method(look_at(c.global_position,Vector3.UP))
		#tween2.tween_method(look_at.bind(Vector3.UP), Vector3(-1, 0, -1), Vector3(1, 0, -1), 7)
	#_get_next_point()


func _move_to_next_point(point):
	print("Starting move to " + str(point))
	var tween = get_tree().create_tween()
	tween.tween
	tween.tween_property(self, "global_position", point.global_position, 7)
	await tween.finished
	print("FINISHED")
	order += 1
	_get_next_point()


func _get_next_point():
	print("Getting point order " + str(order))
	_move_to_next_point(self.get_child(order))
