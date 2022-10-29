extends Node3D

## A GDSCript template used for items that can have more complex mechanic (keypad, ...)
##
## A more detailes comment TODO


var node_interact: Node
## The object hat inicialized interaction (should be player)
var passed_object: Node
## Used for checking if item is being interacted at this moment
var isNowInteract: bool = false
## Get position of where the camera should move to
var camera_pos: Node
## Get position of where the camera should look towards to
var camera_look: Node
## Item name used for displaying item name in interaction
var item_name: String = ""
## Used for displaying action name when look at (Pick up, Use, ..)
var item_interaction_name: String = ""
## Combines interaction names and item name for final tooltip
var item_interaction: String

func _ready():
	node_interact = $interactive_object
	camera_pos = $CameraPos
	camera_look = $CameraLook
	item_interaction = item_interaction_name + " " + item_name

## Logic of the item being used
func _used():
	if isNowInteract == false:
		isNowInteract = true
		passed_object.DisableInputsAndCameraMoveLookTarget(camera_pos.get_global_position(), camera_look.get_global_position())

## Trigged for leaving item interaction
func _input(event):
	if event.is_action_pressed("Jump") and isNowInteract == true:
		_used_quit()
		
## Logic of the leaving interaction
func _used_quit():
		passed_object.EnableInputsAndCameraToNormal()
		passed_object = null
		isNowInteract = false

## Special function required for interaction between GDScript and C#
func message_update():
	var msg:String = node_interact.msgObject.GetMessage()
	if msg == "msg_get_use_action_text":
		node_interact.msgObject.SetStringData(item_interaction)
	if msg == "msg_get_interactive_object_name":
		node_interact.msgObject.SetStringData("placeholder")
	if msg == "msg_use_action":
		passed_object = node_interact.msgObject.GetNodeData()
		self._used()
