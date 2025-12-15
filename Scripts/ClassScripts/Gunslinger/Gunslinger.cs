using Godot;
using System;
using System.Collections.Generic;

public partial class Gunslinger : Character
{
    [Export] public Area2D ShootArea;
    public List<Character> charactersIn = [];
    public bool healingShots;
    public override void _Ready()
    {
        base._Ready();
        ShootArea.BodyEntered += ShootAreaEnter;
        ShootArea.BodyExited += ShootAreaExit;
    }

    private void ShootAreaEnter(Node2D Body)
    {
        if (Body is Character c)
        {
            charactersIn.Add(c);
        }
    }

    private void ShootAreaExit(Node2D Body)
    {
        if (Body is Character c && charactersIn.Contains(c))
        {
            charactersIn.Remove(c);
        }
    }
    
}
