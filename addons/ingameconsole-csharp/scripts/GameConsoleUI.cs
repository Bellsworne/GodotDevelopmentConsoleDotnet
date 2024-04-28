using Godot;
using System;

namespace InGameConsole
{

    public partial class GameConsoleUI : Control
    {

        [Export] private RichTextLabel _outputLabel;
        [Export] private LineEdit _inputField;
        
        
        public override void _EnterTree()
        {
            GameConsole.ConsoleUi = this;
            GameConsole.GetCommands();
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("accept") && _inputField.HasFocus())
            {
                SubmitInput();
            }
        }

        private void SubmitInput()
        {
            Print($"> {_inputField.Text}\n");
            GameConsole.RunCommand(_inputField.Text);
            Print("\n");
            _inputField.Clear();
        }

        public void Print(string input)
        {
            _outputLabel.AppendText(input);
        }

        public void PrintError(string input)
        {
            _outputLabel.AppendText($"[color=red]{input}[/color]");
        }

        public void PrintWarning(string input)
        {
            _outputLabel.AppendText($"[color=yellow]{input}[/color]");
        }
    }
}