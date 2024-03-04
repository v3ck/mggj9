extends Control

class_name Hud

@export var ability_menu_scene: PackedScene
@export var reward_picker_scene: PackedScene

var editing_unit_code: String = ""
var ability_menu: AbilityMenu

signal edit_clicked(unit_code: String)
signal paused
signal resumed
signal ability_moved_up(unit_code: String, ability_code: String)
signal ability_moved_down(unit_code: String, ability_code: String)
signal ability_equipped(unit_code: String, ability_code: String)
signal ability_unequipped(unit_code: String, ability_code: String)
signal reward_picked(ability_code: String)

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

func _on_yuka_card_edit_clicked():
	edit_clicked.emit("YUKA")

func _on_mikan_card_edit_clicked():
	edit_clicked.emit("MIKAN")

func _on_kotori_card_edit_clicked():
	edit_clicked.emit("KOTORI")

func open_ability_menu(unit: UnitResource):
	paused.emit()
	editing_unit_code = unit.code
	ability_menu = ability_menu_scene.instantiate()
	ability_menu.set_unit(unit)
	ability_menu.ability_moved_up.connect(_on_ability_menu_ability_moved_up)
	ability_menu.ability_moved_down.connect(_on_ability_menu_ability_moved_down)
	ability_menu.ability_equipped.connect(_on_ability_menu_ability_equipped)
	ability_menu.ability_unequipped.connect(_on_ability_menu_ability_unequipped)
	ability_menu.closed.connect(_on_ability_menu_closed)
	add_child(ability_menu)

func update_ability_menu(
	unitAbilities: Array[AbilityResource],
	codexAbilities: Array[AbilityResource]):
	ability_menu.set_unit_abilities(unitAbilities)
	ability_menu.set_available_abilities(codexAbilities)

func open_reward_picker(abilities: Array[AbilityResource]):
	paused.emit()
	var reward_picker = reward_picker_scene.instantiate()
	reward_picker.set_abilities(abilities)
	reward_picker.ability_picked.connect(_on_reward_picker_ability_picked)
	add_child(reward_picker)

func update_score(score: int):
	$ScoreLabel.text = "%d" % score

func _on_ability_menu_ability_moved_up(ability_code: String):
	ability_moved_up.emit(editing_unit_code, ability_code)
	
func _on_ability_menu_ability_moved_down(ability_code: String):
	ability_moved_down.emit(editing_unit_code, ability_code)
	
func _on_ability_menu_ability_equipped(ability_code: String):
	ability_equipped.emit(editing_unit_code, ability_code)
	
func _on_ability_menu_ability_unequipped(ability_code: String):
	ability_unequipped.emit(editing_unit_code, ability_code)

func _on_ability_menu_closed():
	resumed.emit()

func _on_reward_picker_ability_picked(ability_code: String):
	reward_picked.emit(ability_code)
	resumed.emit()
