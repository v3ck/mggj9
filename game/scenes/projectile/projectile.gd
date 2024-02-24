extends Node2D

class_name Projectile

@export var speed: float = 240.0

func fire(start_location: Vector2, end_location: Vector2, texture: Texture2D):
	$Sprite.texture = texture
	var duration = start_location.distance_to(end_location) / speed
	position = start_location
	var tween = create_tween()
	tween.tween_property(self, "position", end_location, duration)
	$Timer.start(duration)

func _on_timer_timeout():
	queue_free()
