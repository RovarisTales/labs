extends CharacterBody2D

var speed := 400.0
var dash_speed = 800
var direction: Vector2
var acceleration: float = 10.0
var movement_input: Vector2
var lambda = 1
var deacceleration:float = 5.0


var dash_timer = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	direction = Vector2.ZERO
	velocity = Vector2.ZERO
	movement_input = Vector2.ZERO
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	
	pass

func _physics_process(delta: float) -> void:
	get_movement_input()
	
	update_velocity(0,delta)
	update_velocity(1,delta)
	
	move_and_slide()

func get_movement_input() -> void:
	movement_input = Vector2(
		int(Input.is_action_pressed("move_right")) - int(Input.is_action_pressed("move_left")),
		int(Input.is_action_pressed("move_down")) - int(Input.is_action_pressed("move_up"))
	)

func update_velocity(index, delta):
	var target_velocity = speed * movement_input[index]
	
	var velocity_diferential = target_velocity - velocity[index]
	
	var acceleration_rate = acceleration if abs(target_velocity) > 0.1 else deacceleration
	
	var movement = velocity_diferential * acceleration_rate * delta
	
	velocity[index] += movement
	


func _on_area_2d_body_entered(body: Node2D) -> void:
	print("colision body")
	print(body.get_groups())
