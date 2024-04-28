using Godot;
using System;

namespace InGameConsole;

public partial class GameConsoleUI : Control
{

    [Export] private RichTextLabel _outputLabel;
    [Export] private LineEdit _inputField;
    
    
    public override void _EnterTree()
    {
        GameConsole.ConsoleUi = this;
        GameConsole.GetCommands();

        _inputField.TextSubmitted += text => SubmitInput();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("console"))
        {
            Visible = !Visible;
            if (Visible)
            {
                _inputField.Clear();
            }
        }
    }

    private void SubmitInput()
    {
        Print($"> {_inputField.Text}");
        GameConsole.RunCommand(_inputField.Text);
        _inputField.Clear();
    }

    public void Print(string input)
    {
        _outputLabel.AppendText($"{input}\n");
    }

    public void PrintError(string input)
    {
        _outputLabel.AppendText($"[color=red]{input}[/color]\n");
    }

    public void PrintWarning(string input)
    {
        _outputLabel.AppendText($"[color=yellow]{input}[/color]\n");
    }

    public void Clear()
    {
        _outputLabel.Clear();
    }
}