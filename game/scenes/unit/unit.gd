extends Node2D

class_name Unit

var resource: UnitResource

func _ready():
	$Sprites.texture = resource.sprite_sheet
	$AnimationPlayer.play("Idle")
	$HealthBar.set_faction("GOOD" != resource.faction)

func kill():
	$DeathTimer.start(1.0 / GlobalSettings.tick_rate)

func stun(is_stunned: bool):
	$StunSprite.visible = is_stunned

func shield(is_shielded: bool):
	$ShieldSprite.visible = is_shielded

func set_health(current_health: int):
	$HealthBar.update(current_health, resource.default_health)

func _on_death_timer_timeout():
	queue_free()
