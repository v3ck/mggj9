extends Control

class_name AbilityListItem

@export var resource: AbilityResource

signal mouse_selected

func _ready():
	$NameLabel.text = resource.display_name

func highlight(enableHighlight: bool):
	$Highlight.visible = enableHighlight

func _input(event):
	_try_select(event)

func _try_select(inputEvent):
	if not inputEvent.is_action_pressed("LEFT_CLICK"):
		return
	if not Rect2(Vector2(), size).has_point(get_local_mouse_position()):
		return
	mouse_selected.emit()
