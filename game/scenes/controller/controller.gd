extends Node

class_name Controller

@export var spawnResources: Array[SpawnResource]
@export var unitResources: Array[UnitResource]
@export var abilityResources: Array[AbilityResource]
@export var grid: Grid

var unit_scene

var unitResourcesDict: Dictionary

var unitDict: Dictionary

func _init():
	unit_scene = load("res://scenes/unit/unit.tscn")

func _ready():
	for ability in abilityResources:
		$Logic.AddAbility(ability)
	
	for unit in unitResources:
		$Logic.AddUnit(unit)
		unitResourcesDict[unit.code] = unit
		
	for spawn in spawnResources:
		$Logic.AddSpawn(spawn)
	
	$Logic.StartBattle()
	$TurnTimer.start()

func _destroy_unit():
	pass

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
		_destroy_unit()

func _on_logic_unit_moved(id, fromLocation, toLocation, isTeleport):
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
		tween.tween_property(unit, "position", target_position, 1)

func _on_logic_health_changed(id, location, health):
	pass

func _on_logic_status_changed(id, location, status):
	pass

func _on_logic_ability_fired(fromLocation, toLocation, ability):
	pass
