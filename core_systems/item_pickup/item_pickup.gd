class_name item_pickup extends Node3D

## A GDSCript template used for items that can be picked up.
##
## A more detailes comment TODO

## Used for displaying action name when look at (Pick up, Use, ..)
## First letter should be upper case.
@export var item_interaction_name: String = "Pickup"
## Used for displaying item name in interaction (Battery, Ammo...)
## First letter should be lower case.
@export var item_name: String = "battery"
## Ineractive node used for interaction
@export var node_interact: Node
## Node used for the item logic (add health, battery, ...)
## This node needs to have use() function which is called after item is picked up.
@export var use_node: Node
## Speed (in seconds) of item animation while its towards player.
@export var pickup_speed: float = 0.2
## Height to which item is heading to. 0 is bottom of player's legs.
@export var pickup_height: float = 0.8
## Speed at which item hides itself (to simulate being picked up).
@export var hide_speed: float = 0.1
## Sound effect for pick up. After it finished playing the item will queue_free()
@export var sfx: AudioStream

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
	if sfx != null:
		sfx = load("res://objects/read/paper_test/page_flip.wav")
	node_interact = $interactive_object
	timer = Timer.new()
	self.add_child(timer)
	timer.timeout.connect(_on_Timer_timeout)
	timer.one_shot = true
	sfx_node = AudioStreamPlayer.new()
	sfx_node.stream = sfx
	self.add_child(sfx_node)
	sfx_node.finished.connect(_on_audio_stream_player_finished)
	item_interaction = item_interaction_name + " " + item_name

## Logic of the item being used
func _used() -> void:
	if CustomSettings.debug_oalar:
		Logging.info(self, "Picked up %s" % [item_name])
	var player_position = passed_object.get_global_position()
	var player_height = player_position.y + pickup_height
	sfx_node.play()
	timer.start(hide_speed)
	tween_position = create_tween()
	tween_position.tween_property(self, "global_position", Vector3(player_position.x, player_height, player_position.z), pickup_speed)
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


func _on_Timer_timeout() -> void:
	self.hide()


func _on_audio_stream_player_finished() -> void:
	self.queue_free()
