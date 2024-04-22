extends Node


# Called when the node enters the scene tree for the first time.
func _ready():
	print(GameMaster.GetBuild())
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func testing(a:float) -> void:
	print("test:" + str(a))
