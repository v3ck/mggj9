extends Control

class_name RewardPicker

signal ability_picked(ability_code: String)

func set_abilities(abilities: Array[AbilityResource]):
	$AbilityList.refresh(abilities)

func _on_accept_button_button_up():
	if not $AbilityList.has_selection:
		return
	ability_picked.emit($AbilityList.selected_ability_code)
	queue_free()

func _on_ability_list_selection_changed():
	$AcceptButton.disabled = not $AbilityList.has_selection
