extends Node


var item_name: String = "drawer 1"
var item_interaction_name: String = "Open"
var tween_position: Tween
var state: bool = false
var opening: bool = false


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func use():
	if opening == false:
		opening = true
		var x = get_parent().position.x
		var y = get_parent().position.y
		var z = get_parent().position.z
		if state:
			tween_position = create_tween()
			tween_position.tween_property(self.get_parent(), "position", Vector3(x - 1.5, y, z), 0.8)
			state = false
		else:
			tween_position = create_tween()
			tween_position.tween_property(self.get_parent(), "position", Vector3(x + 1.5, y, z), 0.8)
			state = true
		$Timer.start(0.8)


func _on_timer_timeout():
	opening = false
