using System;
using System.Collections.Generic;
using Godot;

public class Commands
{
    public static Commands Instance = new();
    
    public delegate bool Command(string[] args);
    private Dictionary<string, Command> _commands = new();

    public Commands()
    {
        // Add default commands
        AddCommand(new []{"quit", "exit"}, Quit);
        AddCommand(new []{"clear", "cls"}, Clear);
        AddCommand("echo", Echo);
        AddCommand("version", PrintVersions);
        AddCommand("toggle_console", _ => 
        { 
            Console.Instance.ToggleConsole();
            return true;
        });
        AddCommand("set_console_font_size", args =>
        {
            int newSize = Convert.ToInt32(args[0]);
            if (newSize < 6)
                Console.Instance.WriteError("The console font size can't be less than 6.");
            else if (newSize > 50)
                Console.Instance.WriteError("The console font size can't be more than 50.");
            else
            {
                Console.Instance.SetFontSize(newSize);
                Console.Instance.WriteLine($"Set console font size to {newSize}.");
                return true;
            }
            
            return false;
        });
    }

    public bool GetCommand(string name, out Command command)
    {
        return _commands.TryGetValue(name, out command);
    }

    public static bool Quit(string[] args)
    {
        Console.Instance.GetTree().Quit();
        return true;
    }

    public static bool Clear(string[] args)
    {
        Console.Instance.Clear();
        return true;
    }

    public static bool Echo(string[] args)
    {
        Console.Instance.WriteLine(args.Join(" "));
        return true;
    }

    public bool PrintVersions(string[] args)
    {
        Console.Instance.WriteLine(ProjectSettings.GetSetting("application/config/name").AsString());
        Console.Instance.WriteLine($"On Godot {Engine.GetVersionInfo()["string"]}");
        return true;
    }
    
    public bool IsCommandNameValid(string name) => !name.Contains(' ');
    
    public void AddCommand(string name, Command command)
    {
        if (!IsCommandNameValid(name))
        {
            Console.Instance.WriteError($"\"{name}\" is an invalid name for a command.");
            return;
        }

        if (_commands.ContainsKey(name))
        {
            Console.Instance.WriteError($"\"{name}\" is already a command.");
            return;
        }
        
        _commands.Add(name, command);
    }

    public void AddCommand(string[] names, Command command)
    {
        foreach (string name in names)
            AddCommand(name, command);
    }
}
