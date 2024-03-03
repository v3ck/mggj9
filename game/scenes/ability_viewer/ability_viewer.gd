extends Control

class_name AbilityViewer

func display(abilityResource: AbilityResource):
	$ConditionLabel.text = abilityResource.condition_text
	$ChargeLabel.text = abilityResource.charge_text
	$EffectLabel.text = abilityResource.effect_text
	$AbilityPointLabel.text = "%d" % abilityResource.default_cost
	$ChargePointLabel.text = "%d" % abilityResource.max_charge
