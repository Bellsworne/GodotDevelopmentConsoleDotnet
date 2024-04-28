using Godot;
using System;


namespace InGameConsole
{
    [Tool]
    public partial class Main : EditorPlugin
    {
        private const string GAME_CONSOLE_UI_PATH = "res://addons/ingameconsole-csharp/scenes/GameConsole.tscn";
        public override void _EnablePlugin()
        {
            AddAutoloadSingleton("GameConsole", GAME_CONSOLE_UI_PATH);
        }
        
        public override void _DisablePlugin()
        {
            RemoveAutoloadSingleton("GameConsole");
        }
        
    }
}
