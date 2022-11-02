extends Button

var report_issue_node: Node


func _on_pressed():
	var x = self.get_parent().get_parent().get_parent().get_parent().get_children()
	for child in x:
		if child.name == "report_bug":
			report_issue_node = child
			report_issue_node.show()
