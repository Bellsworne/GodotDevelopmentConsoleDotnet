using Godot;
using System;
using InGameConsole;

public partial class deleteme : Node
{
	
	
	private float myFloat = 5f;

	public override void _EnterTree()
	{
		GameConsole.SetContext(this);
	}

	[Command]
	private void Print(string input)
	{
		GameConsole.Print(input);
	}
	
	[Command(CommandName = "Hello")]
	private void HelloCommand()
	{
		GameConsole.Print("Hello!");
	}
	
	[Command]
	private static void StaticPrint(string input)
	{
		GameConsole.Print(input);
	}
	
	[Command(CommandName = "StaticHello")]
	private static void StaticHelloCommand()
	{
		GameConsole.Print("Hello!");
	}
}
