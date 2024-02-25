extends Node2D

class_name Projectile

func fire(start_location: Vector2, end_location: Vector2, texture: Texture2D, speed: float):
	$Sprite.texture = texture
	var actual_speed = speed * GlobalSettings.tile_size * GlobalSettings.tick_rate
	var duration = start_location.distance_to(end_location) / actual_speed
	position = start_location
	var tween = create_tween()
	tween.tween_property(self, "position", end_location, duration)
	$Timer.start(duration)

func _on_timer_timeout():
	queue_free()
