extends Control

class_name Hud

@export var ability_menu_scene: PackedScene
@export var reward_picker_scene: PackedScene
@export var help_scene: PackedScene

var editing_unit_code: String = ""
var ability_menu: AbilityMenu

var is_user_paused: bool = false

var help_page: HelpPage

signal edit_clicked(unit_code: String)
signal paused
signal resumed
signal stepped
signal speed_normal
signal speed_fast
signal ability_moved_up(unit_code: String, ability_code: String)
signal ability_moved_down(unit_code: String, ability_code: String)
signal ability_equipped(unit_code: String, ability_code: String)
signal ability_unequipped(unit_code: String, ability_code: String)
signal reward_picked(ability_code: String)
signal reward_requested
signal sound_toggled(is_on: bool)
signal quit

func _ready():
	$SoundButton.button_pressed = not GlobalSettings.sound_on

func update_unit_health(code: String, health: int):
	var update_func = func(card): _update_card_health(card, health)
	match code:
		"YUKA":
			update_func.call($YukaCard)
		"MIKAN":
			update_func.call($MikanCard)
		"KOTORI":
			update_func.call($KotoriCard)

func _update_card_health(card: UnitCard, health: int):
	card.set_health(health)

func update_unit_ability_points(code: String, amount: int):
	var update_func = func(card): _update_card_ability_points(card, amount)
	match code:
		"YUKA":
			update_func.call($YukaCard)
		"MIKAN":
			update_func.call($MikanCard)
		"KOTORI":
			update_func.call($KotoriCard)

func _update_card_ability_points(card: UnitCard, amount: int):
	card.set_ability_points(amount)

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

func update_round(rnd: int):
	$RoundLabel.text = "%d" % rnd

func update_rewards(count: int):
	$RewardLabel.text = "%d Unclaimed" % count	
	$RewardButton.disabled = (0 == count)

func _on_ability_menu_ability_moved_up(ability_code: String):
	ability_moved_up.emit(editing_unit_code, ability_code)
	
func _on_ability_menu_ability_moved_down(ability_code: String):
	ability_moved_down.emit(editing_unit_code, ability_code)
	
func _on_ability_menu_ability_equipped(ability_code: String):
	ability_equipped.emit(editing_unit_code, ability_code)
	
func _on_ability_menu_ability_unequipped(ability_code: String):
	ability_unequipped.emit(editing_unit_code, ability_code)

func _on_ability_menu_closed():
	if is_user_paused:
		return
	resumed.emit()

func _on_reward_picker_ability_picked(ability_code: String):
	reward_picked.emit(ability_code)
	if is_user_paused:
		return
	resumed.emit()

func _on_step_button_button_up():
	stepped.emit()

func _on_play_button_button_up():
	speed_normal.emit()
	$PauseButton.button_pressed = false

func _on_fast_forward_button_button_up():
	speed_fast.emit()
	$PauseButton.button_pressed = false

func _on_pause_button_toggled(toggled_on: bool):
	is_user_paused = toggled_on
	if toggled_on:
		paused.emit()
	else:
		resumed.emit()

func _on_reward_button_button_up():
	reward_requested.emit()

func _on_help_button_button_up():
	help_page = help_scene.instantiate()
	help_page.exited.connect(_on_help_page_exited)
	add_child(help_page)
	paused.emit()

func _on_sound_button_toggled(toggled_on):
	sound_toggled.emit(!toggled_on)

func _on_quit_button_button_up():
	quit.emit()

func _on_help_page_exited():
	remove_child(help_page)
	help_page.queue_free()
	if is_user_paused:
		return
	resumed.emit()
