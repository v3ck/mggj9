extends Node2D

class_name UnitCard

@export var unitResource: UnitResource

func _ready():
	$Portrait.texture = unitResource.portrait_sprite
	$NameLabel.text = unitResource.display_name
