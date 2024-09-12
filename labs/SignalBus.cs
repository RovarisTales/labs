using Godot;
using System;

public partial class SignalBus : Node
{
	public static SignalBus Instance { get; private set; }

	[Signal]
	public delegate void BombExplodedEventHandler(Vector2 pos);
	
	[Signal]
	public delegate void StartGameEventHandler();
	
	[Signal]
	public delegate void OpenShopEventHandler();

	public override void _Ready()
	{
		Instance = this;
	}
}
