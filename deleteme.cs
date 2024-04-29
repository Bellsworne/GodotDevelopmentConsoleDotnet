using Godot;
using InGameConsole;

public partial class deleteme : Node
{
	private string _printPrefix = "Instanced: ";
	private string _helloMessage = "Hello!";

	[Command]
	private void Print(string input)
	{
		GameConsole.Print(_printPrefix + input);
	}
	
	[Command(CommandName = "Hello")]
	private void HelloCommand()
	{
		GameConsole.Print(_helloMessage);
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

	[Command]
	private static void Square(int num)
	{
		GameConsole.Print((num * num).ToString());
	}

	[Command]
	private static void NodeTest(Node test)
	{
		GameConsole.Print(test.Name);
	}
}
