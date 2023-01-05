extends HTTPRequest


# Called when the node enters the scene tree for the first time.
func _ready():
	print("requesting")
	#self.connect("request_completed", self, "_on_request_completed")
	#HTTPRequest.pressed.connect(_on_import_scene_pressed)
	request("http://192.168.50.16:1337/api/v1")
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_request_completed(result, response_code, headers, body):
	print("recieved")
	print(result)
	#print(body)``
	print(typeof(body))
	print(response_code)
	_save_image(body)


func _save_image(content):
	var file = FileAccess.open("res://texture.png", FileAccess.WRITE)
	file.store_buffer(content)
	print("saved")
