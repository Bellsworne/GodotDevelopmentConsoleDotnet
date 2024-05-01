using Godot;

namespace InGameConsole;

[Tool]
public partial class Main : EditorPlugin
{
    private const string GameConsoleUiPath = "res://addons/development_console_dotnet/scenes/game_console.tscn";
    public override void _EnablePlugin()
    {
        AddAutoloadSingleton("GameConsole", GameConsoleUiPath);

        
        InputEventKey _inputEventKey = new();
        _inputEventKey.Keycode = Key.Enter;
        InputMap.AddAction("accept");
        InputMap.ActionAddEvent("accept", _inputEventKey);
        
        // InputEventKey _inputEventKey = new();
        // _inputEventKey.Keycode = Key.Enter;
        // InputMap.AddAction("accept");
        // InputMap.ActionAddEvent("accept", _inputEventKey);
        // _inputEventKey.Keycode = Key.Asciitilde;
        // InputMap.AddAction("console");
        // InputMap.ActionAddEvent("console", _inputEventKey);
    }
    
    public override void _DisablePlugin()
    {
        RemoveAutoloadSingleton("GameConsole");
        
        InputMap.EraseAction("accept");
        InputMap.EraseAction("console");
    }
}
