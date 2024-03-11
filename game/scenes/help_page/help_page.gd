extends Control

class_name HelpPage

signal exited

func _on_ok_button_button_up():
	exited.emit()
