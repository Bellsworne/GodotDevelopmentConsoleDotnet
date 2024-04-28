using Godot;
using System;
using InGameConsole;

public partial class deleteme : Node, ICommandable
{
	
	
	private float myFloat = 5f;
	
	[Command]
	private static void MyCommand()
	{
		
	}
	
	[Command(CommandName = "Hello")]
	private static void HelloCommand()
	{
		
	}

	public void Register()
	{
		GameConsole.RegisterInstance(this);
	}

	public void Unregister()
	{
		GameConsole.UnregisterInstance(this);
	}
}
