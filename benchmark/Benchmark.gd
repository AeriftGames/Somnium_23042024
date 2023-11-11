extends Node

@export var url: String = "http://aeriftgames.eu:5000/api/v1/tests/add"

var unique_id: String
var client_name: String = "Oalar"
var tos: bool
var gpu: String
var cpu_name: String
var cpu_count: int
var os: String = OS.get_name() + " " + OS.get_version()
var build: String = "build1"
var level: String = "Somniumlevel1"
var godot_version: String = Engine.get_version_info().string


signal post_benchmark_test_received(code: int)


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	unique_id = OS.get_unique_id()
	var len: int
	len = unique_id.length()
	unique_id = unique_id.substr(1, len - 2)
	gpu = RenderingServer.get_video_adapter_name()
	cpu_name = OS.get_processor_name()
	cpu_count = OS.get_processor_count()
	

func send_test(build: String, levelname: String, quality: String, fps_min: int, fps_max: int, fps_avg: int):
	print("BENCHMARK: SEND TEST")
	self.get_parent().FinishBenchmarkPresset_End(1)


func server_check():
	self.get_parent().FinishBenchmarkPresset_End(1)


func _send_test() -> void:
	post_benchmark_test_received.emit(1)
	pass
	var test_data: Dictionary
	var headers: Array = ["Content-Type: application/json"]
	test_data = {
		"unique_id": unique_id,
		"client_name": client_name,
		"tos": tos,
		"gpu": gpu,
		"cpu_name": cpu_name,
		"cpu_count": cpu_count,
		"build": build,
		"level": level,
		"godot_version": godot_version,
		"os": os
	}
	var query = JSON.stringify(test_data)
	#var query = JSON.stringify(data_to_send)
	$HTTPRequest.request(url, headers, 2, query)


func _on_http_request_request_completed(result, response_code, headers, body) -> void:
	print("complete")
	#var json = JSON.parse(body.get_string_from_utf8())
	#print(json.result)

