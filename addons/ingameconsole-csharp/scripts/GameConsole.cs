using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InGameConsole;

public static class GameConsole
{
    private static GameConsoleUI _consoleUI;
    private static Dictionary<string, (string name, MethodBase method)> _commands = new(StringComparer.OrdinalIgnoreCase);
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
                    _commands.Add(commandName, (commandName, method));
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
    public static bool SetContext(string NodePath)
    {
        var obj = _consoleUI.GetTree().Root.GetNodeOrNull(NodePath);
        return SetContext(obj);
    }

    [Command]
    public static void CurrentContext()
    {
        Print(_context.GetPath());
    }

    [Command]
    public static void ListChildren()
    {
        Print(string.Join("\n", _context.GetChildren().Select(child => child.Name + " : " + child.GetType().FullName)));
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
            return (method.name, method.method, args);
        }
        return null;
    }

    public static bool RunCommand(string input)
    {
        var command = GetCommandFromString(input);
        if (command is null) return false;
        
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
            var expectedParameters = command.method.GetParameters();
            
            Print($"Usage: {command.commandName} {string.Join(" ", expectedParameters.Select(param => $"<{param}>"))}");
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

public class CommandAttribute : Attribute
{
    public string CommandName;
}
