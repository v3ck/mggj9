extends Node2D

class_name Main

func _init():
	GlobalSettings.tick_rate = 2.0
	GlobalSettings.tile_size = 24.0
	GlobalSettings.x_pixels = 480
	GlobalSettings.y_pixels = 270

func _ready():
	if GlobalSettings.sound_on:
		$AudioPlayer.play()
	$Controller.is_audio_playing = GlobalSettings.sound_on

func _on_controller_game_over(is_victory, _final_round, _score, is_quit):
	GlobalSettings.has_played = true
	GlobalSettings.last_result = is_victory
	if is_quit:
		get_tree().change_scene_to_file("res://scenes/home/home.tscn")
	else:
		$GameOverTimer.start()

func _on_game_over_timer_timeout():
	get_tree().change_scene_to_file("res://scenes/home/home.tscn")
