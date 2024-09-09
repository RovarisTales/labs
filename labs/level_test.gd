extends Node2D
@export var player_scene: PackedScene

const SIZE = 15
const DIRECTIONS = [Vector2(0,1),Vector2(0,-1),Vector2(1,0),Vector2(-1,0)]
var grid: Array

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	print(grid)
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass






	


func _on_tile_timer_timeout() -> void:
	pass


	
	
