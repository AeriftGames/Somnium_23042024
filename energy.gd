extends Panel

@export var total_energy: int

var used_energy: int = 0
var remaining_energy: int = 0
var tween: Tween


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_value_changed(self, self)
	_connect_signals()
	_shrink_and_hide()
	tween = create_tween()
	tween.tween_property(self, "size", Vector2(994, 556), 1.1)
	tween.tween_callback(self.test)


func _shrink_and_hide() -> void:
	self.size = Vector2(10, 10)
	$CenterContainer.size = Vector2(10, 554)
	$CenterContainer.hide()
	$CenterContainer.size = Vector2(10, 10)
	$HBoxContainer.hide()


func test():
	$CenterContainer.show()
	tween = create_tween()
	tween.tween_property($CenterContainer, "size", Vector2(991, 554), 0.4)
	tween.tween_callback(self.test2)


func test2():
	$HBoxContainer.show()
	tween = create_tween()
	tween.tween_property($HBoxContainer, "size", Vector2(277, 40), 0.4)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
	
	
func _connect_signals() -> void:
	var x = $CenterContainer/HBoxContainer.get_children()
	for c in x:
		c.value_changed.connect(_value_changed)
	

func _value_changed(one, two):
	used_energy = 0
	remaining_energy = 0
	var x = $CenterContainer/HBoxContainer.get_children()
	for c in x:
		used_energy += c.current_energy
	$HBoxContainer/UsedEnergyValue.text = str(used_energy)
	remaining_energy = total_energy - used_energy
	$HBoxContainer/RemaningEnergyValue.text = str(remaining_energy)
	if remaining_energy == 0:
		for c in x:
			c.locked = true
	else:
		for c in x:
			c.locked = false


func _on_exit_button_pressed() -> void:
	print("EXIT ENERGY")
	get_parent().get_parent()._hide_console(false)
	self.queue_free()

