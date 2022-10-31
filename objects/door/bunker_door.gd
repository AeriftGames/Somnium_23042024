extends Node3D

var tween_left
var tween_right
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func open():
	tween_left = create_tween()
	tween_right = create_tween()
	#tween_right.tween_property($left_door, "position", Vector3(0, 0, 0), 1)
	tween_left.tween_property($left_door, "position", Vector3(1.525, $left_door.position.y, $left_door.position.z), 4)
	#tween_left.tween_property($left_door, "position", Vector3(-1.604, -0.014, 0), 1)
	tween_right.tween_property($right_door, "position", Vector3(-1.525, $right_door.position.y, $right_door.position.z), 4)
