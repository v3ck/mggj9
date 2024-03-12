extends Node

class_name Controller

@export var spawnResources: Array[SpawnResource]
@export var unitResources: Array[UnitResource]
@export var abilityResources: Array[AbilityResource]
@export var grid: Grid
@export var hud: Hud
@export var audio_player: AudioStreamPlayer2D

@export var unit_scene: PackedScene
@export var projectile_scene: PackedScene
@export var ping_scene: PackedScene

@export var normal_tick_rate: float = 2.0
@export var fast_tick_rate: float = 8.0

var unitResourcesDict: Dictionary
var abilityResourcesDict: Dictionary

var unitDict: Dictionary
var hero_id_map: Dictionary

var rewards_earned: Array[Reward] = []

var is_audio_playing: bool = false

signal game_over(is_victory: bool, round: int, score: int)

func _ready():
	_connect_hud()
	
	for ability in abilityResources:
		$Logic.AddAbility(ability)
		abilityResourcesDict[ability.code] = ability
	
	for unit in unitResources:
		$Logic.AddUnit(unit)
		unitResourcesDict[unit.code] = unit
		
	for spawn in spawnResources:
		$Logic.AddSpawn(spawn)
	
	$Logic.StartBattle()
	
	$TurnTimer.wait_time = 1.0 / GlobalSettings.tick_rate
	$TurnTimer.start()

func _connect_hud():
	hud.edit_clicked.connect(_on_hud_edit_clicked)
	hud.paused.connect(_on_hud_paused)
	hud.resumed.connect(_on_hud_resumed)
	hud.stepped.connect(_on_hud_stepped)
	hud.speed_normal.connect(_on_hud_speed_normal)
	hud.speed_fast.connect(_on_hud_speed_fast)
	hud.ability_moved_up.connect(_on_hud_ability_moved_up)
	hud.ability_moved_down.connect(_on_hud_ability_moved_down)
	hud.ability_equipped.connect(_on_hud_ability_equipped)
	hud.ability_unequipped.connect(_on_hud_ability_unequipped)
	hud.reward_picked.connect(_on_hud_reward_picked)
	hud.reward_requested.connect(_on_hud_reward_requested)
	hud.sound_toggled.connect(_on_hud_sound_toggled)
	hud.quit.connect(_on_hud_quit)

func _destroy_unit(id: int):
	if not unitDict.has(id):
		return
	var unit = unitDict[id]
	unitDict.erase(id)
	unit.kill()

func _create_unit(id: int, location: Vector2i, code: String):
	if not unitResourcesDict.has(code):
		return
	var unit = unit_scene.instantiate()
	unit.resource = unitResourcesDict[code]
	unit.position = _location_to_position(location)
	#print(location.x, ", ", location.y)
	#print(unit.position.x, ", ", unit.position.y)
	add_child(unit)
	unitDict[id] = unit
	_link_hero(id, code)

func _link_hero(unit_id: int, unit_code: String):
	match(unit_code):
		"YUKA":
			hero_id_map["YUKA"] = unit_id
		"MIKAN":
			hero_id_map["MIKAN"] = unit_id
		"KOTORI":
			hero_id_map["KOTORI"] = unit_id

func _location_to_position(location: Vector2i):
	return grid.TileToPixel(location.x, location.y) + grid.position

func _on_turn_timer_timeout():
	$Logic.TakeTurn()

func _on_logic_existence_changed(
	id: int,
	location: Vector2i,
	code: String,
	exists: bool):
	if exists:
		_create_unit(id, location, code)
	else:
		_destroy_unit(id)

func _on_logic_unit_moved(id, _fromLocation, toLocation, isTeleport):
	if not unitDict.has(id):
		print("Controller._on_logic_unit_moved() -- id not found")
		return
	var unit = unitDict[id]
	#print(fromLocation.x, ", ", fromLocation.y, " -- ", toLocation.x, ", ", toLocation.y)
	if isTeleport:
		unit.position = _location_to_position(toLocation)
	else:
		_walk_unit(unit, toLocation)

func _walk_unit(unit, to_location):
	var tween = create_tween()
	var target_position = _location_to_position(to_location)
	#print(unit.position.x, ", ", unit.position.y, " -- ", target_position.x, ", ", target_position.y)
	tween.tween_property(
		unit, "position",
		target_position,
		1.0 / GlobalSettings.tick_rate)

func _on_logic_health_changed(id, location, health):
	if not unitDict.has(id):
		print("Controller._on_logic_health_changed() -- id not found")
		return
	var unit = unitDict[id]
	unit.set_health(max(0, health))
	hud.update_unit_health(unit.resource.code, health)
	_ping(location)

func _ping(location):
	var ping = ping_scene.instantiate()
	add_child(ping)
	ping.play(_location_to_position(location))

func _on_logic_status_changed(id, _location, status, is_active):
	if not unitDict.has(id):
		print("Controller._on_logic_status_changed() -- id not found")
		return
	var unit = unitDict[id]
	if "STUN" == status:
		unit.stun(is_active)
	elif "SHIELD" == status:
		unit.shield(is_active)

func _on_logic_ability_fired(fromLocation, toLocation, ability):
	if not abilityResourcesDict.has(ability):
		return
	var ability_resource = abilityResourcesDict[ability]
	var projectile = projectile_scene.instantiate()
	add_child(projectile)
	projectile.fire(
		_location_to_position(fromLocation),
		_location_to_position(toLocation),
		ability_resource.projectile_texture,
		ability_resource.projectile_speed)

func _on_hud_edit_clicked(unit_code: String):
	if not hero_id_map.has(unit_code):
		return
	if not unitResourcesDict.has(unit_code):
		return
	var unit_resource = unitResourcesDict[unit_code]
	hud.open_ability_menu(unit_resource)
	_update_hud_ability_menu(unit_code)

func _ability_codes_to_abilities(ability_codes: Array[String]):
	# Can GDScript do better than this mess?
	var abilities: Array[AbilityResource] = []
	for code in ability_codes:
		if abilityResourcesDict.has(code):
			abilities.append(abilityResourcesDict[code])
	return abilities

func _on_hud_paused():
	$TurnTimer.stop()

func _on_hud_resumed():
	$TurnTimer.start()

func _on_hud_ability_moved_up(unit_code: String, ability_code: String):
	$Logic.MoveUnitAbilityUp(unit_code, ability_code)
	_update_hud_ability_menu(unit_code)

func _on_hud_ability_moved_down(unit_code: String, ability_code: String):
	$Logic.MoveUnitAbilityDown(unit_code, ability_code)
	_update_hud_ability_menu(unit_code)

func _on_hud_ability_equipped(unit_code: String, ability_code: String):
	$Logic.EquipAbility(unit_code, ability_code)
	_update_hud_ability_menu(unit_code)

func _on_hud_ability_unequipped(unit_code: String, ability_code: String):
	$Logic.UnequipAbility(unit_code, ability_code)
	_update_hud_ability_menu(unit_code)

func _update_hud_ability_menu(unit_code: String):
	var unit_ability_codes = $Logic.GetUnitAbilities(unit_code)
	var unit_abilities = _ability_codes_to_abilities(unit_ability_codes)
	var codex_ability_codes = $Logic.GetCodexAbilities(unit_code)
	var codex_abilities = _ability_codes_to_abilities(codex_ability_codes)
	hud.update_ability_menu(unit_abilities, codex_abilities)

func _on_logic_reward_obtained(ability_codes: Array):
	var why_do_i_have_to_do_this: Array[String] = []
	for code in ability_codes:
		why_do_i_have_to_do_this.append(code)
	var abilities = _ability_codes_to_abilities(why_do_i_have_to_do_this)
	var reward = Reward.new()
	reward.abilities = abilities
	rewards_earned.push_back(reward)
	hud.update_rewards(rewards_earned.size())

func _on_hud_reward_picked(ability_code: String):
	$Logic.AddCodexAbility(ability_code)
	offer_reward()

func _on_hud_reward_requested():
	offer_reward()

func offer_reward():
	if rewards_earned.is_empty():
		return
	var rewards = rewards_earned.pop_front()
	hud.update_rewards(rewards_earned.size())
	hud.open_reward_picker(rewards.abilities)

func _on_logic_score_changed(amount):
	hud.update_score(amount)

func _on_logic_round_changed(rnd):
	hud.update_round(rnd)

func _on_hud_stepped():
	$Logic.TakeTurn()

func _on_hud_speed_normal():
	GlobalSettings.tick_rate = normal_tick_rate
	$TurnTimer.wait_time = 1.0 / GlobalSettings.tick_rate

func _on_hud_speed_fast():
	GlobalSettings.tick_rate = fast_tick_rate
	$TurnTimer.wait_time = 1.0 / GlobalSettings.tick_rate

func _on_logic_ability_points_changed(id: int, amount: int):
	if not unitDict.has(id):
		print("Controller._on_logic_ability_points_changed() -- id not found")
		return
	var unit = unitDict[id]
	hud.update_unit_ability_points(unit.resource.code, amount)
	
func _on_hud_sound_toggled(is_on: bool):
	if is_audio_playing:
		audio_player.stream_paused = not is_on
	elif is_on:
		audio_player.play()
		is_audio_playing = true
	GlobalSettings.sound_on = is_on

func _on_logic_game_over(isVictory, final_round, score):
	game_over.emit(isVictory, final_round, score)

func _on_hud_quit():
	game_over.emit(false, 0, 0)
