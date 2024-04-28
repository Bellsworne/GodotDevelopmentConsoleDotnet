using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InGameConsole
{
    public static class GameConsole
    {
        private static GameConsoleUI _consoleUI;
        private static Dictionary<string, MethodBase> _commands = new();
        private static Object _context;

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
                        _commands.Add(attribute.CommandName ?? method.Name, method);
                        GD.Print($"Command: `{attribute.CommandName ?? method.Name}` added.");
                    }
                }
            }
        }

        public static bool SetContext(Object obj)
        {
            if (_context == obj || obj is null) return false;
            _context = obj;
            return true;
        }

        public static void RemoveContext()
        {
            if (_context is null) return;
            _context = null;
        }
        
        public static (string commandName, MethodBase method, List<object> args)? GetCommandFromString(string input)
        {
            string[] splitString = input.Split(" ");
            string commandName = splitString[0];
            List<object> args = new();
            for (int i = 1; i < splitString.Length; i++)
            {
                args.Add(splitString[i]);
            }

            if (_commands.TryGetValue(commandName, out var method))
            {
                return (commandName, method, args);
            }
            return null;
        }

        public static bool RunCommand(string input)
        {
            var command = GetCommandFromString(input);
            if (command is null) return false;
            
            if (command.Value.method.IsStatic)
            {
                command.Value.method.Invoke(null, command.Value.args.ToArray());
            }
            
            else
            {
                if (!command.Value.method.DeclaringType!.IsInstanceOfType(_context))
                {
                    GD.Print($"Invalid context for {command.Value.commandName}");
                    return false;
                }

                command.Value.method.Invoke(_context, command.Value.args.ToArray());
                return true;
            }

            return false;
        }
        
        public static void Print(string input)
        {
            _consoleUI.Print(input);
        }
        
        public static void PrintError()
        {
            
        }
        
        public static void PrintWaring()
        {
            
        }
    }
    
    public class CommandAttribute : Attribute
    {
        public string CommandName;
    }
}

