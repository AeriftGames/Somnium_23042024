extends Panel

var tween: Tween
var tween2: Tween
# Called when the node enters the scene tree for the first time.
func _ready():
	tween = create_tween()
	tween.tween_property(self, "size", Vector2(994, 556), 1.1)
	tween.tween_callback(self.test)

func test():
	$Panel.show()
	tween = create_tween()
	tween.tween_property($Panel, "size", Vector2(303, 547), 0.5)
	tween.tween_callback(self.test2)
	tween.tween_callback(self.test2b)
	
	
func test2():
	$Panel/ScrollContainer.show()
	tween = create_tween()
	tween.tween_property($Panel/ScrollContainer, "size", Vector2(286, 527), 1)

func test2b():
	$Panel/ScrollContainer2.show()
	tween = create_tween()
	tween.tween_property($Panel/ScrollContainer2, "size", Vector2(652, 524), 1)
	

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
