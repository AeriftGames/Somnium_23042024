extends Control

@export var terminal_name: String = "TR-84637"
@export var default_user: String = "admin"
@export var password: String = "asdf"
@export var started: bool = false
@export var locked: bool = true
@export var crt: bool = true
@export var used: bool = true


@onready var scrollbar = $Panel/ScrollContainer.get_v_scroll_bar()

var default_text: String
var input_node: PackedScene = load("res://input.tscn")
var login_node: PackedScene = load("res://login.tscn")
var rng = RandomNumberGenerator.new()
var latest_label: Node
var boot: Array = ["...",
"STARTING USSR TERMINAL",
"",
"CONFIGURING KERNEL",
"kernel.sys = OK",
"kernel.msgmb = 633536",
"kernel.msgmax = 633536",
"kernel.shell = OK",
"",
"CONFIGURING NETWORK",
"network.localhost_ip = 127.0.0.1",
"network_router = OK",
"...",
"BOOT COMPLETE",
""]


func _ready() -> void:
	default_text = "%s@%s:~$ " % [terminal_name, default_user]
	_start()
	if !crt:
		$ColorRect.hide()
	## Provizorni workaround pro zacatke interakce. TODO ODEBRAT
	if used:
		$AudioCRTStart.play()


func _start() -> void:
	if started:
		if locked:
			_spawn_login()
		else:
			_spawn_input()
	else:
		_boot()


func submit(text: String) -> void:
	text = text.to_lower()
	var label = Label.new()
	match text:
		"help":
			_spawn_label("List of available commands:
			restart --- complete restart of this terminal
			clear --- clears terminal
			command3	--- hello world
			
			For confirmation use Y/YES and for decline use N/NO ")
			_spawn_input()
		"restart":
			_spawn_label("Are you sure? Y/N?")
			_spawn_input()
		"y":
			_restart()
		"clear":
			_clear()
			_spawn_input()
		_:
			_spawn_label("Unknown command")
			_spawn_input()


func submit_password(submitted_password: String) -> void:
	var label = Label.new()
	if submitted_password == password:
		_spawn_label("Successful login")
		_spawn_label("")
		_spawn_label("Type help for list of available commands")
		_spawn_input()
	else:
		_spawn_label("Wrong password")
		_spawn_login()


func _process(delta) -> void:
	$Panel/ScrollContainer.scroll_vertical = scrollbar.max_value


func _spawn_login() -> void:
	var x = login_node.instantiate()
	$Panel/ScrollContainer/VBoxContainer.add_child(x)


func _spawn_input() -> void:
	var x = input_node.instantiate()
	$Panel/ScrollContainer/VBoxContainer.add_child(x)


func _spawn_label(text: String, new: bool = true) -> void:
	if new:
		var label = Label.new()
		label.text = text
		label.custom_minimum_size = Vector2(0, 20)
		$Panel/ScrollContainer/VBoxContainer.add_child(label)
		latest_label = label
	else:
		var current_text = latest_label.text
		var new_text = current_text + text
		latest_label.text = new_text


func _boot() -> void:
	rng.randomize()
	var random_number = rng.randf_range(0.5, 1.1)
	for x in boot:
		var label = Label.new()
		label.text = x
		$Panel/ScrollContainer/VBoxContainer.add_child(label)
		random_number = rng.randf_range(0.5, 1.1)
		$Timer.start(random_number)
		await $Timer.timeout
	started = true
	_spawn_login()


func _restart() -> void:
	_spawn_label("RESTARTING")
	$Timer.start(0.9)
	await $Timer.timeout
	_spawn_label(".", false)
	$Timer.start(0.9)
	await $Timer.timeout
	_spawn_label(".", false)
	$Timer.start(0.9)
	await $Timer.timeout
	_spawn_label(".", false)
	_clear()
	started = false
	locked = true
	_start()


func _clear() -> void:
	var all_children = $Panel/ScrollContainer/VBoxContainer.get_children()
	for x in all_children:
		x.queue_free()


func _on_audio_crt_start_finished():
	$AudioCRTLoop.play()
