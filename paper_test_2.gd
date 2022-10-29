extends MeshInstance3D

var isNowInteract = true
var passed_object
var node_interact
var player
var item_name = "Newspaper #7 novinky.cz"
var item_name_label
var item_description = "Rusko nehodlá zaútočit na Ukrajině jadernými zbraněmi, uvedl ve čtvrtek ruský prezident Vladimir Putin. Podle něj by to nemělo ani politický ani vojenský smysl. Poznamenal ale také, že riziko použití jaderných zbraní existuje vždycky, dokud jsou.\n
					„Nepotřebujeme to udělat. Nemělo by to žádný smysl, ani politický, ani vojenský,“ řekl Putin v debatním klubu Valdaj k možnému použití jaderných zbraní.\n
					Pronesl ale také, že „dokud existují jaderné zbraně, vždy existuje nebezpečí jejich použití“.\n
					Rusko opakovaně možný jaderný útok zmiňuje, aby odradilo spojence Ukrajiny, kterou napadla ruská invazní armáda, od dodávek vojenské pomoci.\n
					Putin ale ve čtvrtek tvrdil, že Rusko „nikdy nemluvilo o použití jaderných zbraní“. Vždy šlo prý o reakci na slova západních politiků.\n
					Proti Západu se ruský vládce v debatním klubu vymezoval opakovaně. V tradiční kremelské rétorice ho obviňoval z toho, že na Rusko vyvíjí čím dál větší tlak, aby se mu podvolilo, což se ale prý nestane.\n
					Ve středu Rusko uspořádalo ohlášené cvičení svých jaderných sil. Generální tajemník Severoatlantické aliance Jens Stoltenberg poté zopakoval, že Rusko svým jaderným vyhrožováním NATO od další podpory Ukrajině neodradí.\n"
var item_description_label
var item_labels_node
var showed_description: bool = false
var camera
var sfx
var paper_test


func _ready():
	player = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect")
	node_interact = $interactive_object
	item_labels_node = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect/Control")
	item_name_label = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect/Control/item_name")
	item_description_label = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect/Control/item_description")
	camera = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect/SubViewport/PaperTest/Camera3D")
	paper_test = self.get_node("/root/WorldInterior/FPSCharacter_Interaction/BasicHud/Item_inspect/SubViewport/PaperTest/PaperTest")
	sfx = $AudioStreamPlayer


# Called every frame. 'delta' is the elapsed time since the previous frame.FPSCharacter_Interaction/BasicHud/Item_inspect/SubViewport
func _process(delta):
	pass
	
	
func _input(event):
	if event.is_action_pressed("Jump") and isNowInteract == true:
		_used_quit()
	elif event.is_action_pressed("ShowText") and isNowInteract == true:
		_show_description()
	elif event.is_action_pressed("MouseWheelUp") and isNowInteract == true:
		camera.set_position(Vector3(2, 0, 0))
	elif event.is_action_pressed("MouseWheelDown") and isNowInteract == true:
		camera.set_position(Vector3(3, 0, 0))


func _used():
	sfx.play()
	paper_test.move()
	passed_object.SetInputEnable(false)
	isNowInteract = true
	player.show_inspect(true)
	item_name_label.text = item_name
	item_description_label.text = item_description


func _used_quit():
	player.show_inspect(false)
	passed_object.aSetInputEnable(true)
	passed_object = null
	isNowInteract = false
	item_name_label.text = ""
	item_description_label.text = ""
	showed_description = false
	_show_description()


func _show_description():
	if showed_description == false:
		showed_description = true
		item_labels_node.show()
	else:
		showed_description = false
		item_labels_node.hide()


func message_update():
	var msg:String = node_interact.msgObject.GetMessage()
	if msg == "msg_get_use_action_text":
		node_interact.msgObject.SetStringData(item_name)
	if msg == "msg_get_interactive_object_name":
		node_interact.msgObject.SetStringData("placeholder")
	if msg == "msg_use_action":
		passed_object = node_interact.msgObject.GetNodeData()
		_used()
