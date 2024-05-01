using Godot;

namespace InGameConsole;

[Tool]
public partial class Main : EditorPlugin
{
    private const string GameConsoleUiPath = "res://addons/development_console_dotnet/scenes/game_console.tscn";
    public override void _EnablePlugin()
    {
        AddAutoloadSingleton("GameConsole", GameConsoleUiPath);
    }
    
    public override void _DisablePlugin()
    {
        RemoveAutoloadSingleton("GameConsole");
    }
}
