﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        AddCommand("help", args =>
        {
            Console.Instance.WriteLine($"Commands: {string.Join(", ", _commands.Keys)}");
            return true;
        });
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
        AddCommand("play_game", args =>
        {
            Console.Instance.GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
            return true;
        });
        AddCommand("kill", args =>
        {
            // if (Console.Instance.GetNodeOrNull("/root/Game/Player") is Player player)
            // {
            //     Console.Instance.WriteLine("Ouch.", Colors.Green);
            //     player.Damage(100);
            //     return true;
            // }
            // Console.Instance.WriteLine("No player in the tree!", Colors.Red);

            var player = Console.Instance.GetTree().GetNodesInGroup("Player");
            if (player.Count != 0)
            {
                Console.Instance.WriteLine("Ouch.", Colors.Green);
                ((Player)player[0]).Damage(100);
                return true;
            }
            
            Console.Instance.WriteLine("No player in the tree!", Colors.Red);
            return false;
        });
        AddCommand("kill_enemies", args =>
        {
            // var enemies = Console.Instance.GetTree().Root.FindChildren("*", "Enemy");
            var enemies = Console.Instance.GetTree().GetNodesInGroup("Enemy");
            foreach (var enemy in enemies)
                ((Enemy) enemy).Damage(((Enemy) enemy).MaxHP);
            Console.Instance.WriteLine($"Killed {enemies.Count} enemies.. cheating mf 🤢", Colors.Green);
            return true;
        });
        AddCommand("change_hp", args =>
        {
            if (args.Length == 0)
            {
                Console.Instance.WriteLine("You need to provide an amount, dummy.", Colors.Red);
                return false;
            }
            
            var player = Console.Instance.GetTree().GetFirstNodeInGroup("Player") as Player;
            if (player is null)
            {
                Console.Instance.WriteLine("No players!", Colors.Red);
                return false;
            }

            if (float.TryParse(args[0], out var amount))
            {
                player.Damage(amount);
                Console.Instance.WriteLine($"{(amount < 0 ? "Healed" : "Lost")} {amount} HP!", Colors.Green);
                return true;
            }
            
            Console.Instance.WriteLine("The amounts gotta be a real number!", Colors.Red);
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
