extends Node


var item_name: String = "Battery"
var item_interaction_name: String = "Pick up"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func use():
	print("Battery picked up")