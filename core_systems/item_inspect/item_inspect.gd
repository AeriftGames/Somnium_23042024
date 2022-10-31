extends Control

var rotating: bool = false
var prev_mouse_position
var next_mouse_position
var player
var inspect_node
var isNowInteract = false
var item_name_label
var item_description_label
var active_item
var description_active = false
var spawn_animation_finished = false
var facing_front_text = false #TODO


func _ready():
	self.hide()
	$Control.hide()
	look_changed(true)


func _process(delta):
	var texture = $SubViewport.get_texture()
	$Sprite2D.texture = texture
	_used(delta)


func inspect(state, item):
	if state:
		self.show()
		var r = $SubViewport.create_view(item)
		inspect_node = r
		isNowInteract = true
		active_item = item
		$spawn_animation.start(0.4)


func _show_description(state = false):
	item_name_label = self.get_node("/root/worldlevel/FPSCharacter_Interaction/Item_inspect/Control/item_name")
	item_description_label = self.get_node("/root/worldlevel/FPSCharacter_Interaction/Item_inspect/Control/item_description")
	if description_active == false and state == true:
		description_active = true
		$Control.show()
		item_name_label.text = active_item.item_name
		item_description_label.text = active_item.item_inspect_description
	else:
		description_active = false
		$Control.hide()
		item_name_label.text = ""
		item_description_label.text = ""


func _used(delta):
	if spawn_animation_finished and isNowInteract:
		if (Input.is_action_just_pressed("Rotate")):
			rotating = true
			prev_mouse_position = get_viewport().get_mouse_position()
		if (Input.is_action_just_released("Rotate")):
			rotating = false
		if rotating == true:
			next_mouse_position = get_viewport().get_mouse_position()
			inspect_node.rotate_y((next_mouse_position.x - prev_mouse_position.x) * 1 * delta)
			inspect_node.rotate_z(-(next_mouse_position.y - prev_mouse_position.y) * 1 * delta)
			prev_mouse_position = next_mouse_position


func look_changed(state):
	facing_front_text = state
	if state:
		$VBoxContainer/Label2.show()
	else:
		$VBoxContainer/Label2.hide()
		_show_description(false)


func _input(event):
	if event.is_action_pressed("Jump") and isNowInteract:
		active_item._used_quit()
		isNowInteract = false
		inspect_node.queue_free()
		self.hide()
		_show_description(false)
	elif event.is_action_pressed("ShowText") and isNowInteract and facing_front_text:
		_show_description(true)


func _on_spawn_animation_timeout():
	spawn_animation_finished = true
