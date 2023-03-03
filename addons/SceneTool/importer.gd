@tool
extends Control

var button: Node
var buttonrequest: Node
var httprequest: Node
var log: String
var show_all_nodes: bool


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button = get_node("VBoxContainer/ImportScene")
	button.pressed.connect(_on_import_scene_pressed)
	buttonrequest = get_node("VBoxContainer/RequestButton")
	buttonrequest.pressed.connect(_on_request_button_pressed)
	httprequest = get_node("HTTPRequest")
	httprequest.request_completed.connect(_on_request_completed)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_import_scene_pressed() -> void:
	var rootnode = get_tree().get_edited_scene_root()
	if !show_all_nodes:
		if rootnode.get_class() == "MeshInstance3D":
			log += str(rootnode) + "\n"
	var x = _getchildren(rootnode)
	$VBoxContainer/LabelNodes.text = x
	log = ""
	$VBoxContainer/GridContainer/Name.text = get_tree().get_edited_scene_root().name
	var path = get_tree().get_edited_scene_root().scene_file_path
	$VBoxContainer/GridContainer/Path.text = path
	var file = FileAccess.open(path, FileAccess.READ)
	var scenesize = file.get_length()
	$VBoxContainer/GridContainer/Size.text = str(scenesize) + " bytes"

func _on_request_button_pressed() -> void:
	print("Requesting")
	$HTTPRequest.request("http://185.156.121.239:1337/api/v1")
	

func _getchildren(node: Node):
	for x in node.get_children():
		if x.get_child_count() > 0:
			if !show_all_nodes:
				if x.get_class() == "MeshInstance3D":
					log += str(x) + "\n"
			else:
				log += str(x) + "\n"
			_getchildren(x)
		else:
			if !show_all_nodes:
				if x.get_class() == "MeshInstance3D":
					log += str(x) + "\n"
			else:
				log += str(x) + "\n"
	return log


func _on_show_all_nodes_toggled(button_pressed: bool):
	show_all_nodes = button_pressed
	_on_import_scene_pressed()


func _on_request_completed(result, response_code, headers, body):
	if response_code == 200:
		print("recieved")
	else:
		print("failed")
	_save_image(body)


func _save_image(content):
	var file = FileAccess.open("res://texture.png", FileAccess.WRITE)
	file.store_buffer(content)
	print("saved")


func _on_http_request_request_completed(result, response_code, headers, body):
	pass # Replace with function body.



func _on_button_test_pressed():
	var file = FileAccess.open("res://texture.txt", FileAccess.WRITE)
	file.store_string("contentasdf")
