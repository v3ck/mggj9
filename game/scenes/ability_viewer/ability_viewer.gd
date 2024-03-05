extends Control

class_name AbilityViewer

func display(abilityResource: AbilityResource):
	$NameHeader.text = abilityResource.display_name
	$ConditionLabel.text = abilityResource.condition_text
	if "" == abilityResource.charge_text:
		$ChargeLabel.text = "Always."
	else:
		$ChargeLabel.text = abilityResource.charge_text
	$EffectLabel.text = abilityResource.effect_text
	$AbilityPointLabel.text = "%d" % abilityResource.default_cost
	$ChargePointLabel.text = "%d" % abilityResource.max_charge

func _input(event):
	if event.is_action_released("RIGHT_CLICK"):
		queue_free()
