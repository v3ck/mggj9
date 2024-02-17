extends Node2D

@export var unit: UnitResource

func _ready():
	#$Logic.AddUnit(unit)
	pass

func _on_logic_unit_moved(_id, _fromLocation, _toLocation, _isTeleport):
	#print(id)
	#print(fromLocation)
	#print(toLocation)
	#print(isTeleport)
	pass
