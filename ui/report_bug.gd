extends Control

var url = "http://127.0.0.1:80/api/v1"
var use_ssl = false

var uid: String = str(OS.get_unique_id())
var os: String = str(OS.get_name())
var os_version: String = str(OS.get_version())
var cpu_name: String = str(OS.get_processor_name())
var cpu_count: String = str(OS.get_processor_count())
var issue: String
var reproduce: String


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_http_request_request_completed(result, response_code, headers, body):
	print(response_code)
	print(JSON.parse_string(body.get_string_from_utf8()))
	self.queue_free()
	#print(body)
	pass # Replace with function body.


func _on_button_2_pressed():
	var data_to_send = ["a", "b", "c"]
	var query = {
  "uid": "Songname",
  "os": "artistname",
  "cpu": "oalar",
  "issue": "myissue description"
}
	var json_string = JSON.stringify(query)
	var headers = ["Content-Type: application/json"]
	$HTTPRequest.request(url, headers, use_ssl, HTTPClient.METHOD_POST, json_string)
	pass # Replace with function body.


func _on_submit_pressed():
	issue = $ColorRect/ColorRect/VBoxContainer/HBoxContainer/issue_TextEdit.text
	reproduce = $ColorRect/ColorRect/VBoxContainer/HBoxContainer2/reproduce_TextEdit.text
	print("XX")
	print(issue)
	var query: Dictionary = {
"uid": uid,
"os": os,
"os_version": os_version,
"cpu_name": cpu_name,
"cpu_count": cpu_count,
"issue": issue,
"reproduce": reproduce
}
	var json_string = JSON.stringify(query)
	var headers = ["Content-Type: application/json"]
	$HTTPRequest.request(url, headers, use_ssl, HTTPClient.METHOD_POST, json_string)


func _on_close_pressed():
	self.queue_free()
