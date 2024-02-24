extends Node2D

class_name Unit

var resource: UnitResource

func _ready():
	$Sprites.texture = resource.sprite_sheet
	$AnimationPlayer.play("Idle")

func kill():
	$DeathTimer.start()

func _on_death_timer_timeout():
	queue_free()
