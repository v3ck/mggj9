extends Control

class_name AbilityList

@export var ability_list_item_scene: PackedScene

var selected_ability_code: String
var has_selection: bool = false

signal selection_changed

func refresh(abilities: Array[AbilityResource]):
	_clear()
	var found_selection = false
	for ability in abilities:
		found_selection = found_selection or _add(ability)
	if not found_selection:
		has_selection = false
		selection_changed.emit()

func _clear():
	var items = $Container.get_children()
	for item in items:
		_remove(item)
	
func _add(ability: AbilityResource) -> bool:
	var item = ability_list_item_scene.instantiate()
	item.resource = ability
	item.mouse_selected.connect(_on_item_mouse_selected(item))
	$Container.add_child(item)
	if has_selection and selected_ability_code == ability.code:
		item.highlight()
		return true
	return false

func _remove(item: AbilityListItem):
	$Container.remove_child(item)
	item.mouse_selected.disconnect(_on_item_mouse_selected)
	item.queue_free()

func _on_item_mouse_selected(item: AbilityListItem):
	if has_selection and selected_ability_code == item.resource.code:
		return
	var items = $Container.get_children()
	for otherItem in items:
		(otherItem as AbilityListItem).highlight(false)
	item.highlight(true)
	selected_ability_code = item.resource.code
	has_selection = true
	selection_changed.emit()

