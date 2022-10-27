extends Control


func _ready():
	self.hide()


func show_inspect(state):
	if state:
		self.show()
	else:
		self.hide()
