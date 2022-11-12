extends Panel

var tween: Tween
var tween2: Tween
@export var messages: Array
var messages_dict: Dictionary
var button: PackedScene = load("res://terminal_mail_button.tscn")
var selected_message: TerminalMessage


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_shrink_and_hide()
	_create_messages()
	tween = create_tween()
	tween.tween_property(self, "size", Vector2(994, 556), 1.1)
	tween.tween_callback(self.test)


func _shrink_and_hide() -> void:
	self.size = Vector2(10, 10)
	$Panel.size = Vector2(10, 10)
	$Panel.hide()
	$Panel/ScrollContainer.size = Vector2(10, 10)
	$Panel/ScrollContainer.hide()
	$Panel/ScrollContainer2.size = Vector2(10, 10)
	$Panel/ScrollContainer2.hide()


func test() -> void:
	$Panel.show()
	tween = create_tween()
	tween.tween_property($Panel, "size", Vector2(303, 547), 0.5)
	tween.tween_callback(self.test2)
	tween.tween_callback(self.test2b)
	
	
func test2() -> void:
	$Panel/ScrollContainer.show()
	tween = create_tween()
	tween.tween_property($Panel/ScrollContainer, "size", Vector2(286, 527), 1)


func test2b() -> void:
	$Panel/ScrollContainer2.show()
	tween = create_tween()
	tween.tween_property($Panel/ScrollContainer2, "size", Vector2(652, 524), 1)


func _create_messages() -> void:
	for x in messages:
		var z = button.instantiate()
		z.text = x.get("subject")
		z.msg_text = x.get("message")
		var zz = $Panel/ScrollContainer/VBoxContainer.add_child(z)
		_connect_signals()


func _connect_signals() -> void:
	var x = $Panel/ScrollContainer/VBoxContainer.get_children()
	for c in x:
		c.button_toggled.connect(_on_button_pressed)
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta) -> void:
	pass


func _on_button_pressed(button, state) -> void:
	if state:
		_untoggle_message(button)
		$Panel/ScrollContainer2/VBoxContainer/Subject.text = button.text
		$Panel/ScrollContainer2/VBoxContainer/Message.text = button.msg_text


func _untoggle_message(button) -> void:
	var x = $Panel/ScrollContainer/VBoxContainer.get_children()
	for c in x:
		if c.button_pressed and c != button:
			c.button_pressed = false


func _on_exit_button_pressed() -> void:
	get_parent().get_parent()._hide_console(false)
	self.queue_free()
