extends SubViewport


var tween_position: Tween
var facing_front_text: bool = false


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func create_view(view):
	var new_view = view.inspect_node.instantiate()
	new_view.position = Vector3(0, -2, 0)
	self.add_child(new_view)
	tween_position = create_tween()
	tween_position.tween_property(new_view, "position", Vector3(0, 0, 0), 0.4)
	return new_view


func _on_area_3d_area_entered(area):
	get_parent().look_changed(true)


func _on_area_3d_area_exited(area):
	get_parent().look_changed(false)
