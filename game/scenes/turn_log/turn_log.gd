extends Control

class_name TurnLog

@export var item_count: int
@export var turn_log_item_scene : PackedScene

var items: Array[TurnLogItem]

func _ready():
	for i in range(item_count):
		init_item()

func init_item():
	var turn_log_item = turn_log_item_scene.instantiate()
	$Container.add_child(turn_log_item)
	items.push_back(turn_log_item)

func add_item(unit_name: String, ability_name: String):
	var text = "%s did nothing." % unit_name \
		if ability_name.is_empty() \
		else "%s used %s" % [unit_name, ability_name]
	var turn_log_item = turn_log_item_scene.instantiate()
	turn_log_item.set_text(text)
	$Container.add_child(turn_log_item)
	var old_item = items.pop_front()
	$Container.remove_child(old_item)
	old_item.queue_free()
	items.push_back(turn_log_item)
	
