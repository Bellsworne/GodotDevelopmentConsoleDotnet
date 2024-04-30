using Godot;
using System;
using InGameConsole;

public partial class Player : Node3D
{
	private float _health = 100;

	[Command(Description = "Gets the Health value of Player")]
	private void GetPlayerHealth()
	{
		GameConsole.Print($"Player health is currently: {_health}");
	}

	[Command(Description = "Sets the Health value of Player")]
	private void SetPlayerHealth(float amt)
	{
		_health = amt;
		GameConsole.Print($"Player health set to: {_health}");
	}

	[Command(Description = "Moves the player")]
	private void MovePlayer(Vector3 pos)
	{
		Position = pos;
	}
	
	[Command]
	[Command(CommandName = "damage")]
	private void DamagePlayer(float amt)
	{
		_health -= amt;
		GameConsole.Print(_health.ToString());
	}
}
