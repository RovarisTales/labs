extends Area2D


func _on_body_entered(body: Node2D) -> void:
	print("colision area")
	print(body.get_groups())
	if body.is_in_group("bombs"):
		pass
