extends Button

func _on_pressed() -> void:
	SignalBus.StartGame.emit()
