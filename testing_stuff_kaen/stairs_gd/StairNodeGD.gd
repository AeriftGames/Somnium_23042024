extends Node3D

var allstair_node:Node = null

@export var number_of_stairs = 1:
	set(new_number_of_stairs):
		number_of_stairs = new_number_of_stairs
		if allstair_node != null:
			_generate_stairs(number_of_stairs)
			
@export var stair_prefab:PackedScene = load("res://testing_stuff_kaen/stairs_gd/stair_classic1.tscn") as PackedScene


@export var enable_world_col:bool:
	set(new_enable_world_col):
		enable_world_col = new_enable_world_col

@export var enable_player_col:bool:
	set(new_enable_player_col):
		enable_player_col = new_enable_player_col

func _ready() -> void:
	#if Engine.is_editor_hint():
	#	if get_tree().edited_scene_root != null:
	#		self.set_owner(get_tree().edited_scene_root)
	#else:
	#	pass
	
	start_init()
	

func start_init():
	await get_tree().create_timer(0.5).timeout
	if Engine.is_editor_hint():
		for a in get_children():
			a.queue_free()
			
		#if get_node("AllStairs") != null:
		#	get_node("AllStairs").queue_free()
			
		await get_tree().process_frame

		if get_tree().edited_scene_root != null:
			allstair_node = Node3D.new()
			add_child(allstair_node)
			allstair_node.name = "AllStairs"
			allstair_node.set_owner(get_tree().edited_scene_root)
	
			_generate_stairs(number_of_stairs)
	
	else:
		
		await get_tree().process_frame
			
		if get_tree() != null:
			if not Engine.is_editor_hint():
				if get_node("AllStairs") != null:
					for child in get_node("AllStairs").get_children():
						child.set_stair_world_collision(enable_world_col)
						child.set_stair_player_collision(enable_player_col)
	
func _generate_stairs(new_number_of_stairs:int):
	if not Engine.is_editor_hint():
		return
	
	if get_tree() != null:
		if allstair_node != null:
			for child in allstair_node.get_children():
				child.queue_free()
	else:
		return

	await get_tree().process_frame
	
	#var stair_prefab:PackedScene = load("res://testing_stuff_kaen/stairs_gd/stair_classic1.tscn") as PackedScene
	
	for i in new_number_of_stairs:
		print("generate "+str(i)+" stair")
		var new_stair = stair_prefab.instantiate() as stair_classic
		if allstair_node != null:
			allstair_node.add_child(new_stair)
			new_stair.set_owner(get_tree().edited_scene_root)
			new_stair.name = "stair_"+str(i)
			new_stair.set_stair_world_collision(enable_world_col)
			new_stair.set_stair_player_collision(enable_player_col)
			
			if i > 0:
				var end_pos:Vector3 = allstair_node.get_child(i-1).call("get_stair_end_global_position") as Vector3
				new_stair.global_position = end_pos
			
