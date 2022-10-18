extends MeshInstance3D


var pos = self.get_position()
var pressed = false
 
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_area_3d_input_event(camera, event, position, normal, shape_idx):
	if get_parent().locked == true:
		pass
	elif pressed != true:
		if Input.is_action_pressed("mouseClickLeft"):
			pressed = true
			self.set_position(Vector3(pos.x,pos.y,0.085))
			self.get_parent()._play_sound()
			$Timer.start(0.1)
			get_parent().input(self.name)


func _on_timer_timeout():
	self.set_position(Vector3(pos.x,pos.y,0.135))
	pressed = false
