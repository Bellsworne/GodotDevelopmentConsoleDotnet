using Godot;

namespace InGameConsole;

public partial class GameConsoleUI : Control
{

    [Export] private RichTextLabel _outputLabel;
    [Export] private TextEdit _inputField;

    private CodeHighlighter _highlighter;
    
    
    public override void _EnterTree()
    {
        GameConsole.ConsoleUi = this;
        GameConsole.GetCommands();
        
        _highlighter = (CodeHighlighter)_inputField.SyntaxHighlighter;
        
        _highlighter.AddColorRegion("(", ")", Colors.Red);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("accept") && _inputField.HasFocus()) SubmitInput();
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