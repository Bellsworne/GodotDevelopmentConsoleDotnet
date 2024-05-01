using System.Net.Mime;
using Godot;

namespace InGameConsole;

public partial class GameConsoleUI : Control
{

    [Export] private RichTextLabel _outputLabel;
    [Export] private TextEdit _inputField;
    [Export] private Label _contextLabel;
    [Export] private string _motd = "Type `help` for a list of commands.";

    private bool _canTween = true;
    
    public override void _EnterTree()
    {
        GameConsole.ConsoleUi = this;
        Print(_motd);
        
        GameConsole.GetCommands();
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("accept") && _inputField.HasFocus())
        {
            GetViewport().SetInputAsHandled();
            SubmitInput();
        }
        
        if (@event.IsActionPressed("console"))
        {
            GetViewport().SetInputAsHandled();
            if (Visible)
            {
                HideConsole();
            }
            else
            {
                ShowConsole();
            }
        }
    }

    private void ShowConsole()
    {
        if (_canTween)
        {
            _canTween = false;
            Visible = true;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(GetNode<Panel>("Panel"), "modulate:a", 1, 0.1f);
            tween.SetEase(Tween.EaseType.InOut);
            tween.Play();
            tween.Finished += () =>
            {
                _canTween = true;
                _inputField.GrabFocus();
            };
        }
    }

    private void HideConsole()
    {
        if (_canTween)
        {
            _canTween = false;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(GetNode<Panel>("Panel"), "modulate:a", 0, 0.1f);
            tween.SetEase(Tween.EaseType.InOut);
            tween.Play();
            tween.Finished += () =>
            {
                Visible = false;
                _canTween = true;
            };
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
    
    public void SetContext(string path)
    {
        _contextLabel.Text = $"{path} $:";
    }
}