@tool
extends HBoxContainer

signal value_changed(button, state)

@export var node_name: String
@export_enum("no", "low", "medium", "high") var current_energy

var no_energy = load("res://terminal_energy.tres")
var low_energy = load("res://terminal_energy_low.tres")
var medium_energy = load("res://terminal_energy_medium.tres")
var high_energy = load("res://terminal_energy_high.tres")
var current_enegy_label: String
var maximum_energy: int = 3
var locked: bool = false


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$TextureRect.texture = no_energy


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	$TextureRect/NodeName.text = node_name
	_set_texture()


func _on_texture_rect_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.pressed:
		if event.button_index == 1:
			if !locked:
				var available_energy = get_parent().get_parent().get_parent().remaining_energy
				_add_energy()
				$AudioStreamPlayer.play()
			else:
				$AudioStreamPlayer2.play()
		elif event.button_index == 2:
			_remove_energy()
			$AudioStreamPlayer.play()


func _add_energy():
	if current_energy == 3:
		current_energy = 0
	else:
		current_energy += 1
	emit_signal("value_changed", self, self)
	
	
func _remove_energy():
	if current_energy > 0:
		current_energy -= 1
		emit_signal("value_changed", self, self)


func _set_texture():
	match current_energy:
		0:
			$TextureRect.texture = no_energy
			$TextureRect/EnergyCount.text = "0"
		1:
			$TextureRect.texture = low_energy
			$TextureRect/EnergyCount.text = "1"
		2:
			$TextureRect.texture = medium_energy
			$TextureRect/EnergyCount.text = "2"
		3:
			$TextureRect.texture = high_energy
			$TextureRect/EnergyCount.text = "3"
