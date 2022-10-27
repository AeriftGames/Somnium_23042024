extends MeshInstance3D

var isNowInteract = true
var passed_object
var node_interact
var player


func _ready():
	player = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect")
	node_interact = $interactive_object


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _used():
	passed_object.SetInputEnable(false)
	isNowInteract = true
	player.show_inspect(true)


func _used_quit():
	player.show_inspect(false)
	passed_object.SetInputEnable(true)
	passed_object = null
	isNowInteract = false


func message_update():
	var msg:String = node_interact.msgObject.GetMessage()
	if msg == "msg_get_use_action_text":
		node_interact.msgObject.SetStringData("Rotate item")
	if msg == "msg_get_interactive_object_name":
		node_interact.msgObject.SetStringData("placeholder")
	if msg == "msg_use_action":
		passed_object = node_interact.msgObject.GetNodeData()
		_used()
