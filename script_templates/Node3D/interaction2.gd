extends Node3D

## A GDSCript template used for items that can be TODO
##
## A more detailes comment TODO

## Variable used for interacting with interactive_object required for interaction.
var node_interact: Node
## The object hat inicialized interaction (should be player)
var passed_object: Node
## Used for checking if item is being interacted at this moment
var isNowInteract: bool = false
## Item name used for displaying item name in interaction
var item_name: String = "Battery"
## Used for displaying action name when look at (Pick up, Use, ..)
var item_interaction_name: String = "Pick up"
## Combines interaction names and item name for final tooltip
var item_interaction: String


func _ready():
	node_interact = $interactive_object

## Logic of the item being used
func _used():
	if isNowInteract == false:
		isNowInteract = true
		passed_object.SetInputEnable(false)
		item_interaction = item_interaction_name + " " + item_name

## Trigged for leaving item interaction
func _input(event):
	if event.is_action_pressed("Jump") and isNowInteract == true:
		_used_quit()
		
## Logic of the leaving interaction
func _used_quit():
		passed_object.SetInputEnable(true)
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
