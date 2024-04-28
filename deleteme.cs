using Godot;
using System;
using InGameConsole;

public partial class deleteme : Node
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
}
