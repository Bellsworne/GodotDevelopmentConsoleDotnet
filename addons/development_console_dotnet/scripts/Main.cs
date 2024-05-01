using Godot;
using Godot.Collections;

namespace InGameConsole;

[Tool]
public partial class Main : EditorPlugin
{
    private const string GameConsoleUiPath = "res://addons/development_console_dotnet/scenes/game_console.tscn";
    public override void _EnablePlugin()
    {
        AddAutoloadSingleton("GameConsole", GameConsoleUiPath);

        if (!ProjectSettings.HasSetting("input/accept"))
        {
            var input = new Dictionary();
            input.Add("deadzone", 0.5f);
            input.Add("events", new Array()
            {
                new InputEventKey() {Keycode = Key.Enter}
            });
            ProjectSettings.SetSetting("input/accept", input);
            ProjectSettings.Save();
        }
        
        if (!ProjectSettings.HasSetting("input/console"))
        {
            var input = new Dictionary();
            input.Add("deadzone", 0.5f);
            input.Add("events", new Array()
            {
                new InputEventKey() {Keycode = Key.Asciitilde}
            });
            ProjectSettings.SetSetting("input/console", input);
            ProjectSettings.Save();
        }
        
        // This forces the editor to restart,
        // which is needed for some stupid reason to actually update the input settings
        // -- Nevermind this makes the plugin not actually enable. Now I am sad
        //EditorInterface.Singleton.RestartEditor();
    }
    
    public override void _DisablePlugin()
    {
        RemoveAutoloadSingleton("GameConsole");
    }
}
