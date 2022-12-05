extends Node3D

@export var always_on: bool
@export var flashing: bool

@onready var light = $Light
@onready var material = light.get_active_material(0)
@onready var light_on = load("res://light_on.tres")
@onready var light_off = load("res://light_off.tres")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	print(light)
	print(material)
	#material.emission_enabled = self.emission_enabled
	if always_on:
		_switch_on()
	else:
		_switch_off()
	if flashing:
		pass


func _switch_on():
	light.set_surface_override_material(0, light_on)


func _switch_off():
	light.set_surface_override_material(0, light_off)
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
