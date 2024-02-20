extends Node

class_name Controller

@export var spawnResources: Array[SpawnResource]
@export var unitResources: Array[UnitResource]
@export var abilityResources: Array[AbilityResource]
@export var grid: Grid

var unitResourcesDict: Dictionary

func _ready():
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
	var unit = Unit.new()
	unit.resource = unitResourcesDict[code]
	unit.location = grid.TileToPixel(location.x, location.y)
	add_child(unit)

func _on_logic_existence_changed(id: int, location: Vector2i, code: String, exists: bool):
	print("HI")
	if exists:
		_create_unit(id, location, code)
	else:
		_destroy_unit()

func _on_turn_timer_timeout():
	$Logic.TakeTurn()
