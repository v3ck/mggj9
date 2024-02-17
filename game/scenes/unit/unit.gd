extends Node2D

@export var resource: UnitResource

func _ready():
	$Sprites.texture = resource.sprite_sheet
	$AnimationPlayer.play("Idle")
