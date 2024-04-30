using Godot;

namespace InGameConsole;

public partial class GameConsoleUI : Control
{

    [Export] private RichTextLabel _outputLabel;
    [Export] private TextEdit _inputField;

    private CodeHighlighter _highlighter;
    private AnimationPlayer _animation;
    
    
    public override void _EnterTree()
    {
        GameConsole.ConsoleUi = this;

        _animation = GetNode<AnimationPlayer>("AnimationPlayer");
        _animation.SpeedScale = 2;
        
        GameConsole.GetCommands();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("accept") && _inputField.HasFocus()) SubmitInput();
        
        if (Input.IsActionJustPressed("console"))
        {
            if (Visible)
            {
                _animation.Play("main");
                _inputField.Clear();
            }
            else
            {
                _animation.PlayBackwards("main");
                _inputField.GrabFocus();
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