extends Node3D

## A GDSCript template used for items that can be picked up.
##
## A more detailes comment TODO


## Variable used for interacting with interactive_object required for interaction.
var node_interact: Node
## The object hat inicialized interaction (should be player)
var passed_object: Node
## Item name used for displaying item name in interaction
var item_name: String = "Battery"
## Used for displaying action name when look at (Pick up, Use, ..)
var item_interaction_name: String = "Pick up"
## Combines interaction names and item name for final tooltip
var item_interaction: String
## Tween used for anymating pickup anymation
var tween_position: Tween


func _ready():
	node_interact = $interactive_object
	item_interaction = item_interaction_name + " " + item_name

## Logic of the item being used
func _used():
	var player_position = passed_object.get_global_position()
	var player_height = player_position.y + 0.5
	$AudioStreamPlayer.play()
	$interactive_object/StaticBody3D/CollisionShape3D.disabled = true
	tween_position = create_tween()
	tween_position.tween_property(self, "position", Vector3(player_position.x, player_height, player_position.z), 0.1)
	$Despawn.start(0.1)

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


func _on_hide_timeout():
	self.hide()


func _on_audio_stream_player_finished():
	self.queue_free()
