using Godot;
using System;

namespace InGameConsole
{

    public partial class GameConsoleUI : Control
    {
        public override void _EnterTree()
        {
            GameConsole.ConsoleUi = this;
            GameConsole.GetCommands();
        }
    }
}