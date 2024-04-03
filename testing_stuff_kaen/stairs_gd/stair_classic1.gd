@tool
class_name stair_classic
extends Node3D

@export var stair_id:int = 0

func get_stair_end_global_position() -> Vector3:
	var node:Node3D = get_node("Cube/StairEnd") as Node3D
	return node.global_position
	
func set_stair_world_collision(new_enable:bool):
	get_node("StaticBody3D_WorldPhysics/CollisionShape3D").set_deferred("disabled",!new_enable)
	#a.set_collision_layer_value(1,new_enable)
	#print("aaa "+str(new_enable))
	
func set_stair_player_collision(new_enable:bool):
	get_node("StaticBody3D_PlayerPhysics/CollisionPolygon3D").set_deferred("disabled",!new_enable)
	#a.set_collision_layer_value(2,new_enable)
	#print("bbb "+str(new_enable))
