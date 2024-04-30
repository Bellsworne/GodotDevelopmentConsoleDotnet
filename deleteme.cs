using Godot;
using InGameConsole;

public partial class deleteme : Node
{
	[Command]
	private void Square(int num)
	{
		GameConsole.Print((num * num).ToString());
	}

	[Command]
	private static void NodeTest(Node test)
	{
		GameConsole.Print(test.Name);
	}
	
	[Command]
	private static void Move2D(Vector2 vector)
	{
		GameConsole.Print(vector.ToString());
	}

	[Command]
	private static void Move3D(Vector3 vector)
	{
		GameConsole.Print(vector.ToString());
	}
	
}
