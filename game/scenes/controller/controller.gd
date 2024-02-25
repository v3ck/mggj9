extends Node

class_name Controller

@export var spawnResources: Array[SpawnResource]
@export var unitResources: Array[UnitResource]
@export var abilityResources: Array[AbilityResource]
@export var grid: Grid

@export var unit_scene: PackedScene
@export var projectile_scene: PackedScene

var unitResourcesDict: Dictionary
var abilityResourcesDict: Dictionary

var unitDict: Dictionary

func _ready():	
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

func _location_to_position(location: Vector2i):
	return grid.TileToPixel(location.x, location.y) + grid.position

func _on_turn_timer_timeout():
	$Logic.TakeTurn()

func _on_logic_existence_changed(id: int, location: Vector2i, code: String, exists: bool):
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
		var tween = create_tween()
		var target_position = _location_to_position(toLocation)
		#print(unit.position.x, ", ", unit.position.y, " -- ", target_position.x, ", ", target_position.y)
		tween.tween_property(unit, "position", target_position, 1.0 / GlobalSettings.tick_rate)

func _on_logic_health_changed(id, _location, health):
	if not unitDict.has(id):
		print("Controller._on_logic_health_changed() -- id not found")
		return
	var unit = unitDict[id]
	unit.set_health(max(0, health))

func _on_logic_status_changed(_id, _location, _status):
	pass

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
