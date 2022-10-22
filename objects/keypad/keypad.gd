extends Node3D

@export var combination = 1235
var keypadtext
var max_Characters = false
var special_state = false
var locked = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _play_sound():
	$KeyPress.play()


func input(key):
	if key == "clear":
		_clear()
	elif key == "enter":
		_enter()
	else:
		if special_state == true:
			_clear()
			special_state = false
		_update_text(key)
	pass

func _update_text(text):
	if max_Characters == false:
		var x = $KeypadText.text
		x = x + str(text)
		$KeypadText.text = str(x)
		if len(x) == 4:
			max_Characters = true
	else:
		pass

func _clear():
	$KeypadText.text = ""
	max_Characters = false


func _enter():
	if str(combination) == $KeypadText.text:
		$KeypadText.text = "OK"
		locked = true
		$OK.play()
	else:
		$KeypadText.text = "ERROR"
		special_state = true
		$Error.play()
