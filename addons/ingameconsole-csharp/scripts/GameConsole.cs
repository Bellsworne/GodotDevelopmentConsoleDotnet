using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace InGameConsole
{
    public static class GameConsole
    {
        private static GameConsoleUI _consoleUI;
        private static Dictionary<string, MethodBase> _commands = new();
        private static List<ICommandable> _instanceRegistry = new();

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

        public static void RegisterInstance(ICommandable instance)
        {
            if (_instanceRegistry.Contains(instance)) return;
            _instanceRegistry.Add(instance);
        }

        public static void UnregisterInstance(ICommandable instance)
        {
            if (!_instanceRegistry.Contains(instance)) return;
            _instanceRegistry.Remove(instance);
        }

        
        public static void GetCommands()
        {
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes<CommandAttribute>();

                    foreach (var attribute in attributes)
                    {
                        _commands.Add(attribute.CommandName ?? method.Name, method);
                        GD.Print($"Method {method.Name} added.");
                    }
                }
            }
        }
    }    
    
    public class CommandAttribute : Attribute
    {
        public string CommandName;
    } 
}

