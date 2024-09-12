extends Node


func _ready() -> void:
	SignalBus.StartGame.connect(start_game)
	
func start_game() -> void:
	print("startei o game")
