extends Node2D
@export var player_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	
	var player = player_scene.instantiate()
	
	player.position = Vector2(128,128)
	add_child(player)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass






	


func _on_tile_timer_timeout() -> void:
	pass


	
	
