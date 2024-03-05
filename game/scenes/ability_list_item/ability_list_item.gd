extends Control

class_name AbilityListItem

@export var resource: AbilityResource
@export var uncommon_label_settings: LabelSettings
@export var rare_label_settings: LabelSettings

signal mouse_selected
signal ability_viewed

func _ready():
	$NameLabel.text = resource.display_name
	if 1 == resource.rarity:
		$NameLabel.label_settings = uncommon_label_settings
	elif 2 == resource.rarity:
		$NameLabel.label_settings = rare_label_settings

func highlight(enableHighlight: bool):
	$Highlight.visible = enableHighlight

func _input(event):
	_try_select(event)
	_try_view(event)

func _try_select(inputEvent):
	if not inputEvent.is_action_pressed("LEFT_CLICK"):
		return
	if not Rect2(Vector2(), size).has_point(get_local_mouse_position()):
		return
	mouse_selected.emit()

func _try_view(inputEvent):
	if not inputEvent.is_action_pressed("RIGHT_CLICK"):
		return
	if not Rect2(Vector2(), size).has_point(get_local_mouse_position()):
		return
	ability_viewed.emit()
	
