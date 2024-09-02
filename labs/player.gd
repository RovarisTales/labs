extends Node2D

var speed := 200.0
var dash_speed = 800
var direction: Vector2
var velocity: Vector2
var acceleration: Vector2
var movement_input: Vector2
var lambda = 1

var dash_timer = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	direction = Vector2.ZERO
	velocity = Vector2.ZERO
	movement_input = Vector2.ZERO
	acceleration = Vector2(0.2,0.2)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	
	get_movement_input()
	
	update_velocity(0,delta)
	update_velocity(1,delta)
	position +=  velocity
	
	position = position.clamp(Vector2.ZERO, Vector2(1920,1080))

func get_movement_input() -> void:
	movement_input = Vector2(
		int(Input.is_action_pressed("move_right")) - int(Input.is_action_pressed("move_left")),
		int(Input.is_action_pressed("move_down")) - int(Input.is_action_pressed("move_up"))
	).normalized()

func update_velocity(index, delta):
	if abs(velocity[index]) < speed and abs(movement_input[index]) != 0:
		velocity[index] += movement_input[index] * acceleration[index] * speed * delta
	elif abs(movement_input[index]) != 0:
		clampf(velocity[index], -speed, speed)
	elif  velocity[index] < -speed and abs(movement_input[index]) != 0:
		velocity[index] = -speed
	elif abs(velocity[index]) > 0 and movement_input[index] == 0:
		if abs(velocity[index]) < lambda:
			velocity[index] = 0
		elif velocity[index] > 0:
			velocity[index] = max(velocity[index] -acceleration[index] * speed * 1.5 * delta, 0)
		elif velocity[index] <0:
			velocity[index] = min(velocity[index] + acceleration[index] * speed * 1.5 * delta, 0)
