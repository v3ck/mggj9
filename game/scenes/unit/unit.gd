extends Node2D

class_name Unit

var resource: UnitResource
var location: Vector2

func _ready():
	$Sprites.texture = resource.sprite_sheet
	$AnimationPlayer.play("Idle")
