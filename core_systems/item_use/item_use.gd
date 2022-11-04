class_name item_use extends Node3D
@icon("res://core_systems/item_use/item_use_icon.svg")

## A GDSCript template used for items that can be picked up.
##
## A more detailes comment TODO


## Used for displaying action name when look at (Pick up, Use, ..)
## First letter should be upper case.
@export var item_interaction_name: String = "Open"
## Used for displaying item name in interaction (Battery, Ammo...)
## First letter should be lower case.
@export var item_name: String = "drawer"
## Node used for the item logic (add health, battery, ...)
## This node needs to have use() function which is called after item is picked up.
@export var use_node: Node
## Sound effect for pick up. After it finished playing the item will queue_free()
@export var sfx: AudioStream
## Select custom interact scene?
@export var custom_node_interact: bool

## Ineractive node scene used for interaction
@onready var node_interact_scene = load("res://core_systems/interactive_system/interactive_object.tscn")

## Ineractive node used for interaction
var node_interact: Node
## The object hat inicialized interaction (should be player)
var passed_object: Node
## Combines interaction names and item name for final tooltip
var item_interaction: String
## Tween used for anymating pickup anymation
var tween_position: Tween
## TODO
var timer: Node
## TODO
var sfx_node: Node


func _ready() -> void:
	node_interact = node_interact_scene.instantiate()
	self.add_child(node_interact)
	node_interact.position = get_parent().position
	use_node = $use
	item_name = use_node.item_name
	item_interaction_name = use_node.item_interaction_name
	item_interaction = item_interaction_name + " " + item_name

## Logic of the item being used
func _used() -> void:
	var player_position = passed_object.get_global_position()
	var player_height = player_position.y + 0.5
	use_node.use()

## Special function required for interaction between GDScript and C#
func message_update() -> void:
	var msg:String = node_interact.msgObject.GetMessage()
	if msg == "msg_get_use_action_text":
		node_interact.msgObject.SetStringData(item_interaction)
	if msg == "msg_get_interactive_object_name":
		node_interact.msgObject.SetStringData("placeholder")
	if msg == "msg_use_action":
		passed_object = node_interact.msgObject.GetNodeData()
		self._used()

