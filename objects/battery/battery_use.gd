extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func use():
	print("Battery picked up")
	
	# Inventory test
	var inventoryItemDataNode = get_node_or_null("../InventoryItemDataNode")
	if(inventoryItemDataNode != null):
		inventoryItemDataNode.call("UseWantAddToInventory",get_parent().scene_file_path)
