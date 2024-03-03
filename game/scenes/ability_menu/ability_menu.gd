extends Control

class_name AbilityMenu

signal ability_moved_up(ability_code)
signal ability_moved_down(ability_code)
signal ability_equipped(ability_code)
signal ability_unequipped(ability_code)

func set_unit_abilities(abilities: Array[AbilityResource]):
	$UnitAbilityList.refresh(abilities)

func set_available_abilities(abilities: Array[AbilityResource]):
	$AvailableAbilityList.refresh(abilities)

func _on_move_up_button_button_up():
	ability_moved_up.emit($UnitAbilityList.selected_ability_code)

func _on_move_down_button_button_up():
	ability_moved_down.emit($UnitAbilityList.selected_ability_code)

func _on_equip_button_button_up():
	ability_equipped.emit($AvailableAbilityList.selected_ability_code)

func _on_unequip_button_button_up():
	ability_unequipped.emit($UnitAbilityList.selected_ability_code)

func _on_unit_ability_list_selection_changed():
	var disable_buttons = not $UnitAbilityList.has_selection
	$MoveUpButton.disabled = disable_buttons
	$MoveDownButton.disabled = disable_buttons
	$UnequipButton.disabled = disable_buttons

func _on_available_ability_list_selection_changed():
	$EquipButton.disabled = not $AvailableAbilityList.has_selection
