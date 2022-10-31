extends OmniLight3D

var state: bool = true
var rng = RandomNumberGenerator.new()


# Called when the node enters the scene tree for the first time.
func _ready():
	rng.randomize()
	_flash()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _flash():
	var random_number = rng.randf_range(0.2, 0.6)
	if state:
		state = false
		#self.hide()
		self.light_energy = 0.010
	else:
		state = true
		#self.show()
		self.light_energy = 0.741
	$Timer.start(random_number)


func _on_timer_timeout():
	_flash()
	pass # Replace with function body.
