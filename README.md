# C# Dev console for Godot 4.2.2

## Installation
___
> - Download the desired release from the Releases page on Github
> - Open your C# Godot 4.2 project and click on AssetLib > Plugins > Import...
> - Select the Release.zip file you downloaded, and click install
> - Add two input actions, `console` and `accept`
> - Enable the plugin
> - Profit

## Usage
___
The console will be added as an autoloaded scene when the plugin is enabled. Simple press the key associated with the `console` input action in game and the console will appear.

- Typing the `help` command will list all static (or "global") commands, as well as any commands that are valid for the context.
- `help -a` will show ALL commands, regardless of context.

### Understanding Context
This console is built around setting the context, aka the instance of a class, to call non-static commands.
You can set context in three ways. 
1. Using the `SetContext` or `cd` command in the console, passing the name of a Node in the scene (you can view nodes by typing `ShowChildren` or `ls`)
2. In a script, you can use `GameConsole.SetContext(this)` passing either `this` to get the instance you are in, or any other instanced class.
3. ~~Using the `ShowTree` or `tree` command to get a UI of the scene tree to select a node.~~ // Not in the a1.0 release.

## Documentation
___
### Adding a command
- In any script, simply add a `[Command]` attribute above any method.
*You can optionally add a an alias with `[Command(CommandName = "MyCommand")]` you can add as attributes as you want, but it will create a new entry in the registry for each* 
- Static methods can be called at any time in the console, without needing to set the context.
- Non-static methods will need the context in the console to be set to an instanced node of the correct type.
