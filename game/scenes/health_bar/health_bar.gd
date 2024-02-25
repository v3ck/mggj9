extends Node2D

class_name HealthBar

func update(current_health: float, max_health: float):
	$CurrentBar.scale.x = ceilf($CurrentBar.size.x * current_health / max_health) / $CurrentBar.size.x
	
func set_faction(isEnemy: bool):
	if isEnemy:
		$CurrentBar.color = Color(168./255, 43./255, 18./255)
	else:
		$CurrentBar.color = Color(148./255, 204./255, 71./255)
