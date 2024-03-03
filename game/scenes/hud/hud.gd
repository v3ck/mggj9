extends Control

class_name Hud

@export var ability_menu_scene: PackedScene

func update_unit(code: String, health: int, ability_points: int):
	var update_func = func(card): _update_card(card, health, ability_points)
	match code:
		"YUKA":
			update_func.call($YukaCard)
		"MIKAN":
			update_func.call($MikanCard)
		"KOTORI":
			update_func.call($KotoriCard)

func _update_card(card: UnitCard, health: int, ability_points: int):
	card.set_health(health)
	card.set_ability_points(ability_points)
