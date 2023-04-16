extends Node

var unique_id: String
var client_name: String = "Oalar"
var tos: bool
var gpu: String
var cpu_name: String
var cpu_count: int
var os: String = OS.get_name() + " " + OS.get_version()
var url: String = "http://aeriftgames.eu:5000/api/v1/tests/add"
var fps_avg: int = 75
var fps_min: int = 10
var fps_max: int = 50
var build: String = "build1"
var level: String = "Somniumlevel1"
var fps_all: Array
var godot_version: String = Engine.get_version_info().string

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	print("Starting")
	unique_id = OS.get_unique_id()
	var len: int
	len = unique_id.length()
	unique_id = unique_id.substr(1, len - 2)
	print(unique_id)
	print(typeof(unique_id))
	gpu = RenderingServer.get_video_adapter_name()
	cpu_name = OS.get_processor_name()
	cpu_count = OS.get_processor_count()
	print(unique_id, gpu, cpu_name, cpu_count)
	print(gpu[0])
	_send_test()


func _send_test() -> void:
	print("Sending test")
	var test_data: Dictionary
	var headers = ["Content-Type: application/json"]
	test_data = {
		"unique_id": unique_id,
		"client_name": client_name,
		"tos": tos,
		"gpu": gpu,
		"cpu_name": cpu_name,
		"cpu_count": cpu_count,
		"build": build,
		"level": level,
		"fps_avg": fps_avg,
		"fps_min": fps_min,
		"fps_max": fps_max,
		"godot_version": godot_version,
		"os": os
	}
	var query = JSON.stringify(test_data)
	#var query = JSON.stringify(data_to_send)
	$HTTPRequest.request(url, headers, 2, query)

	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta) -> void:
	fps_all.append(Engine.get_frames_per_second())
	#print(fps_all.size())
	pass


func _on_http_request_request_completed(result, response_code, headers, body) -> void:
	print("complete")
	#var json = JSON.parse(body.get_string_from_utf8())
	#print(json.result)

## Testing function to test fps.
func _on_timer_timeout():
	var sum = 0
	for e in fps_all:
		sum += e
	print("Average FPS: " + str(sum / fps_all.size()))
	print("Minimum FPS: " + str(fps_all.min()))
	print("Maximum FPS: " + str(fps_all.max()))
	pass # Replace with function body.
