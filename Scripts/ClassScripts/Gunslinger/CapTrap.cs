using Godot;
using System;
using System.Collections.Generic;

public partial class CapTrap : Minion
{
    [Export]
    public Area2D Area;
    [Export]
    public AnimatedSprite2D GunSpot;
    public PrimaryWeapon wep;
    public Character owner;
    private List<Character> targets = new();
    public override void _Ready()
    {
        Area.BodyEntered += OnTrapEnter;    
        Area.BodyExited += OnTrapExit;
        GunSpot.SpriteFrames = wep.SpriteFrames;
    }

    private void OnTrapEnter(Node2D body)
    {
        if (body is Enemy e)
        {
            if (e != owner && !targets.Contains(e))
            {
                targets.Add(e);
            }
        }
    }
    private void OnTrapExit(Node2D body)
    {
        if (body is Enemy e)
        {
            if (targets.Contains(e))
            {
                targets.Remove(e);
            }
        }
    }
}
