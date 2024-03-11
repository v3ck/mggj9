extends Node2D

class_name Home

@export var help_scene: PackedScene

var help_page: HelpPage

func _ready():
	_display_message()

func _display_message():
	if not GlobalSettings.has_played:
		$Canvas/CreditMessage.visible = true
	elif GlobalSettings.last_result:
		$Canvas/WinMessage.visible = true
	else:
		$Canvas/FailMessage.visible = true

func _on_start_button_button_up():
	get_tree().change_scene_to_file("res://scenes/main/main.tscn")

func _on_help_button_button_up():
	help_page = help_scene.instantiate()
	help_page.exited.connect(_on_help_page_exited)
	$Canvas.add_child(help_page)

func _on_quit_button_button_up():
	get_tree().quit()

func _on_help_page_exited():
	$Canvas.remove_child(help_page)
	help_page.queue_free()
