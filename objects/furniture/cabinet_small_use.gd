extends Node

## Creates tween for opening animation
var tween_position: Tween
## Indicated state (false = closed)
var state: bool = false
## Indicates if it's opening at this moment. This is created for forbidding
## forbiding plyaer to interact with the item while it is being used
var opening: bool = false
## TODO
var timer: Node


func _ready() -> void:
	timer = Timer.new()
	self.add_child(timer)
	timer.timeout.connect(_on_timer_timeout)
	timer.one_shot = true
	
## This is logic for opening of an drawer. It is called from the parent script
## called item_use.gd
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
		timer.start(0.8)


func _on_timer_timeout():
	opening = false
