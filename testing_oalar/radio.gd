extends Node3D

var onoff = $CSGBox3D/CSGSphere3D
var tween: Tween
var sensitivity = 1

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	#$CSGBox3D/CSGCylinder3D.look_at()
	$CSGBox3D/CSGCylinder3D.rotate_object_local(Vector3.UP, event.relative.x * sensitivity)


func _onoff_pressed(camera: Node, event: InputEvent, position: Vector3, normal: Vector3, shape_idx: int) -> void:
	if Input.is_action_pressed("mouseClickLeft"):
		tween = create_tween()
		tween.tween_property($CSGBox3D/CSGSphere3D, "position", Vector3(0.4, 0.367, 0.504), 0.05)
		tween.tween_callback(self.test)
		

func test():
	tween = create_tween()
	tween.tween_property($CSGBox3D/CSGSphere3D, "position", Vector3(0.4, 0.367, 0.573), 0.05)
