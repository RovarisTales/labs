extends Node2D
@export var player_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	
	$Player.position = Vector2(128,128)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _on_tile_timer_timeout() -> void:
	pass
