@icon("res://core_systems/item_inspect/item_inspect_icon.svg")
class_name item_inspect extends Node3D

## A GDSCript template used for items that can be TODO
##
## A more detailes comment TODO

## Item name used for displaying item name in interaction
@export var item_name: String = "postcard_test"
## Used for displaying action name when look at (Pick up, Use, ..)
@export var item_interaction_name: String = "Inspect"
## TODO
@export var item_inspect_description: String = "1235"
## 
@export var inspect_node: PackedScene
##
@export var sfx = load("res://objects/read/paper_test/page_flip.wav")
## Variable used for interacting with interactive_object required for interaction.
var node_interact: Node
## The object hat inicialized interaction (should be player)
var passed_object: Node
## Used for checking if item is being interacted at this moment
var isNowInteract: bool = false
## Combines interaction names and item name for final tooltip
var item_interaction: String
## Player node
var player_inspect: Node


func _ready():
	if sfx != null:
		sfx = load("res://objects/read/paper_test/page_flip.wav")
	node_interact = $interactive_object
	item_interaction = item_interaction_name + " " + item_name

## Logic of the item being used
func _used():
	if isNowInteract == false:
		player_inspect = self.get_node("/root/worldlevel/FPSCharacter_Interaction/Item_inspect")
		#$AudioStreamPlayer.play()
		isNowInteract = true
		passed_object.SetInputEnable(false)
		player_inspect.inspect(true, self, false)

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

