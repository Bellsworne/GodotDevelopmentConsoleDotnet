using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InGameConsole;

public static class GameConsole
{
    private static GameConsoleUI _consoleUI;
    private static Dictionary<string, (CommandAttribute attribute, MethodBase method)> _commands = new(StringComparer.OrdinalIgnoreCase);
    private static Node _context;

    public static GameConsoleUI ConsoleUi
    {
        get => _consoleUI;
        set
        {
            if (_consoleUI is not null)
            {
                GD.PrintErr("Trying to set the GameConsoleUI instance, but it is already set.");
                return;
            }
            _consoleUI = value;
            SetContext(_consoleUI.GetTree().Root);
        }
    }
    
    public static void GetCommands()
    {
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes<CommandAttribute>();

                foreach (var attribute in attributes)
                {
                    var commandName = attribute.CommandName ?? method.Name;
                    var newAttribute = new CommandAttribute
                    {
                        CommandName = commandName,
                        Description = attribute.Description
                    };
                    _commands.Add(commandName, (newAttribute, method));
                    GD.Print($"{(method.IsStatic ? "Static" : "Instanced")} Command: `{commandName}` added.");
                }
            }
        }
    }

    public static bool SetContext(Node obj)
    {
        if (_context == obj || obj is null)
        {
            PrintError("Node not found");
            return false;
        }
        _context = obj;
        Print($"Context switched to {_context.GetPath()}");
        return true;
    }

    [Command]
    [Command(CommandName = "cd")]
    public static bool SetContext(string NodePath)
    {
        var obj = _context.GetNodeOrNull(NodePath);
        return SetContext(obj);
    }

    [Command]
    [Command(CommandName = "pwd")]
    public static void CurrentContext()
    {
        Print($"{_context.GetPath()} : [color=green]{_context.GetType().FullName}[/color]");
    }

    [Command]
    [Command(CommandName = "ls")]
    [Command(CommandName = "dir")]
    public static void ListChildren()
    {
        Print(string.Join("\n", _context.GetChildren().Select(child => $"{child.Name} : [color=yellow]{child.GetType().FullName}[/color]")));
    }

    [Command]
    public static void Destroy()
    {
        if (_context == _context.GetTree().Root)
        {
            PrintError("Cannot destroy /root");
            return;
        }
        var deleteContext = _context;
        SetContext(_context.GetParent());
        deleteContext.QueueFree();
        Print("Node destroyed");
    }

    [Command]
    public static void Help()
    {
        Print(string.Join("\n", _commands.Select(command => $"{(command.Value.method.IsStatic ? "" : $"{(command.Value.method.DeclaringType!.IsInstanceOfType(_context) ? "[color=green]" : "[color=yellow]")}(from '{command.Value.method.DeclaringType!.FullName}' context)[/color] ")}{command.Value.attribute.CommandName} {string.Join(" ", command.Value.method.GetParameters().Select(param => $"[color=cyan]<{param}>[/color]"))}{(string.IsNullOrWhiteSpace(command.Value.attribute.Description) ? "" : $"\t[color=slate_gray]#{command.Value.attribute.Description}[/color]")}")));
    }

    [Command(Description = "Clear the screen")]
    public static void Clear()
    {
        _consoleUI.Clear();
    }
    
    public static (string commandName, MethodBase method, List<object> args)? GetCommandFromString(string input)
    {
        var splitString = input.Split(" ");
        var commandName = splitString[0];
        List<object> args = new();
        for (var splitIndex = 1; splitIndex < splitString.Length; splitIndex++)
        {
            args.Add(splitString[splitIndex]);
        }

        if (_commands.TryGetValue(commandName, out var method))
        {
            return (method.attribute.CommandName, method.method, args);
        }
        return null;
    }

    public static bool RunCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var command = GetCommandFromString(input);
        if (command is null)
        {
            PrintError("Unknown command");
            return false;
        }
        
        if (command.Value.method.IsStatic)
        {
            return ExecuteCommand(null, command.Value);
        }

        if (command.Value.method.DeclaringType!.IsInstanceOfType(_context))
        {
            return ExecuteCommand(_context, command.Value);
        }

        PrintError($"Invalid context for '{command.Value.commandName}', context needs to be instance of type '{command.Value.method.DeclaringType.FullName}'");
        return false;
    }

    private static bool ExecuteCommand(object? obj, (string commandName, MethodBase method, List<object> args) command)
    {
        try
        {
            command.method.Invoke(obj, command.args.ToArray());
            return true;
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
            Print($"Usage: {command.commandName} {string.Join(" ", command.method.GetParameters().Select(param => $"[color=cyan]<{param}>[/color]"))}");
        }

        return false;
    }
    
    public static void Print(string input)
    {
        _consoleUI.Print(input);
    }
    
    public static void PrintError(string input)
    {
        _consoleUI.PrintError(input);
    }
    
    public static void PrintWarning(string input)
    {
        _consoleUI.PrintWarning(input);
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommandAttribute : Attribute
{
    public string CommandName;
    public string Description;
}
