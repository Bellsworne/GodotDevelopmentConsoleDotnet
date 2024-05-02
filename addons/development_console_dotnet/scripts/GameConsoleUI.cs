using System;
using System.Linq;
using System.Net.Mime;
using Godot;
using Godot.Collections;

namespace InGameConsole;

public partial class GameConsoleUI : Control
{

    [ExportCategory("Console")]
    [Export] private Panel _consolePanel;
    [Export] private RichTextLabel _outputLabel;
    [Export] private TextEdit _inputField;
    [Export] private Label _contextLabel;

    [ExportCategory("Tree view")] 
    [Export] private Panel _treePanel;
    [Export] private Tree _tree;
    
    [Export] private string _motd = "Type `help` for a list of commands.";

    private bool _consoleCanTween = true;
    private bool _treeCanTween = true;
    
    public override void _EnterTree()
    {
        GameConsole.ConsoleUi = this;
        GameConsole.GetCommands();
        GameConsole.RunCommand("tree");
        
        Print(_motd);
        
        var item = _tree.CreateItem();
        item.SetText(0, "tree_root");
        SetupTree(GetTree().Root, _tree.GetRoot());
        _tree.ItemActivated += TreeItemActivated;
        
        GetTree().NodeRemoved += SceneTreeNodeRemoved;
        GetTree().NodeAdded += SceneTreeNodeAdded;
        GetTree().NodeRenamed += SceneTreeNodeRenamed;
    }

    private void TreeItemActivated()
    {
        GameConsole.RunCommand($"cd {GetTreeItemPath(_tree.GetSelected())}");
    }

    private void SceneTreeNodeRemoved(Node node)
    {
        // TODO: Update tree
        // GameConsole.PrintWarning($"{node.GetPath()} was removed");
        
        var nodePath = node.GetPath();

        var treeContext = _tree.GetRoot();
        for (var nodePathIndex = 0; nodePathIndex < nodePath.GetNameCount(); nodePathIndex++)
        {
            treeContext = treeContext.GetChildren().SingleOrDefault(child => child.GetText(0) == nodePath.GetName(nodePathIndex));
            if (treeContext == null)
            {
                return;
            }
        }
        
        treeContext.GetParent().RemoveChild(treeContext);
    }
    
    private void SceneTreeNodeAdded(Node node)
    {
        // TODO: Update tree
        // GameConsole.PrintWarning($"{node.GetPath()} was added");
        
        var nodePath = node.GetPath();

        var treeContext = _tree.GetRoot();
        for (var nodePathIndex = 0; nodePathIndex < nodePath.GetNameCount(); nodePathIndex++)
        {
            var newTreeContext = treeContext.GetChildren().SingleOrDefault(child => child.GetText(0) == nodePath.GetName(nodePathIndex));
            if (newTreeContext == null)
            {
                newTreeContext = treeContext.CreateChild();
                newTreeContext.SetTooltipText(0, " ");
                newTreeContext.Collapsed = true;
                newTreeContext.SetText(0, nodePath.GetName(nodePathIndex));
            }

            treeContext = newTreeContext;
        }
    }
    
    private void SceneTreeNodeRenamed(Node node)
    {
        var nodePath = node.GetParent().GetPath();

        var treeContext = _tree.GetRoot();
        for (var nodePathIndex = 0; nodePathIndex < nodePath.GetNameCount(); nodePathIndex++)
        {
            treeContext = treeContext.GetChildren().SingleOrDefault(child => child.GetText(0) == nodePath.GetName(nodePathIndex));
            if (treeContext == null)
            {
                return;
            }
        }

        foreach (var child in treeContext.GetChildren().ToArray())
        {
            treeContext.RemoveChild(child);
        }
        
        SetupTreeChildren(node.GetParent(), treeContext);
        SetContextLabel(node.GetPath());
    }

    private void SetupTreeChildren(Node nodeParent, TreeItem treeParent)
    {
        foreach (var child in nodeParent.GetChildren(true))
        {
            SetupTree(child, treeParent);
        }
    }

    private string GetTreeItemPath(TreeItem item)
    {
        if (item is null) return "";

        if (item.GetParent() == null || item.GetParent().GetText(0) is "tree_root")
        {
            return $"/{item.GetText(0)}";
        }

        string parent = GetTreeItemPath(item.GetParent());

        return $"{parent}/{item.GetText(0)}";
    }

    private void SetupTree(Node current, TreeItem root)
    {
        TreeItem newItem = root.CreateChild();
        newItem.SetTooltipText(0, " ");
        newItem.Collapsed = true;
        newItem.SetText(0, current.Name);

        SetupTreeChildren(current, newItem);
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
        if (_consoleCanTween)
        {
            _consoleCanTween = false;
            Visible = true;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(this, "modulate:a", 1, 0.1f);
            tween.SetEase(Tween.EaseType.InOut);
            tween.Play();
            tween.Finished += () =>
            {
                _consoleCanTween = true;
                _inputField.GrabFocus();
            };
        }
    }

    private void HideConsole()
    {
        if (_consoleCanTween)
        {
            _consoleCanTween = false;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(this, "modulate:a", 0, 0.1f);
            tween.SetEase(Tween.EaseType.InOut);
            tween.Play();
            tween.Finished += () =>
            {
                Visible = false;
                _consoleCanTween = true;
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
    
    public void SetContextLabel(string path)
    {
        _contextLabel.Text = $"{path} $:";
    }

    public void ToggleTree()
    {
        if (_treePanel.Visible && _treeCanTween)
        {
            _treeCanTween = false;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_treePanel, "custom_minimum_size", new Vector2(0, 0), 0.1f);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Elastic);
            tween.Play();
            tween.Finished += () =>
            {
                _treePanel.Visible = false;
                _treeCanTween = true;
            };
        }
        
        if (!_treePanel.Visible && _treeCanTween)
        {
            _treeCanTween = false;
            _treePanel.Visible = true;
            var tween = GetTree().CreateTween();
            tween.TweenProperty(_treePanel, "custom_minimum_size", new Vector2(250, 0), 0.1f);
            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Elastic);
            tween.Play();
            tween.Finished += () =>
            {
                _treeCanTween = true;
            };
        }
    }
}