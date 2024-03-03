extends Control

class_name UnitCard

@export var unitResource: UnitResource

signal edit_clicked

var max_health: int = 0

func _ready():
	$Portrait.texture = unitResource.portrait_sprite
	$NameLabel.text = unitResource.display_name
	max_health = unitResource.default_health
	set_health(max_health)
	set_ability_points(-1)

func _on_edit_button_button_down():
	edit_clicked.emit()

func set_health(health: int):
	$HealthLabel.text = "%d/%d HP" % [health, max_health]

func set_ability_points(ability_points: int):
	$AbilityPointLabel.text = "%d AP" % ability_points
