extends Node2D

class_name Ping

func play(location: Vector2):
	position = location
	$AnimationPlayer.speed_scale = GlobalSettings.tick_rate
	$AnimationPlayer.play("play")
	$Timer.start(1.0 / GlobalSettings.tick_rate)

func _on_timer_timeout():
	queue_free()
