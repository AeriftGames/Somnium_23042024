extends Control


func _ready():
	self.hide()
	pass


func _process(delta):
	var texture = $SubViewport.get_texture()
	$Sprite2D.texture = texture


func show_inspect(state):
	if state:
		self.show()
	else:
		self.hide()
