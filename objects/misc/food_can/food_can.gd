extends Node3D
@tool

@export_enum("Tomato", "Tuna", "Corn") var can_type

var tomato = load("res://objects/misc/food_can/food_can_rajce.tres")
var tuna = load("res://objects/misc/food_can/food_can_tunak.tres")
var corn = load("res://objects/misc/food_can/food_can_kukurice.tres")

# Called when the node enters the scene tree for the first time.
func _ready():
	_set_texture()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	_set_texture()


func _set_texture():
	match can_type:
		0:
			$Cylinder.set_material_override(tomato)
		1:
			$Cylinder.set_material_override(tuna)
		2:
			$Cylinder.set_material_override(corn)
