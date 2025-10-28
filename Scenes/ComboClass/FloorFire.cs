using Godot;
using System;
using System.Collections.Generic;

public partial class FloorFire : Sprite2D
{
    private Timer DamageTimer;
    private Timer DeleteTimer;
    [Export]
    public Area2D DamageArea;
    [Export] public float Damage;
    private List<Character> DamageChars = [];
    public override void _Ready()
    {
        GetParent().MoveChild(this, 0);
        DamageArea.BodyEntered += DamageAreaEnter;
        DamageArea.BodyExited += DamageAreaExit;
        DamageTimer = new Timer()
        {
            Autostart = true,
            OneShot = false,
            WaitTime = 0.5f,
            Name = "DamageTimer"
        };
        DamageTimer.Timeout += DealDamage;
        DeleteTimer = new Timer()
        {
            Autostart = true,
            OneShot = true,
            WaitTime = 3f,
            Name = "DeleteTimer"
        };
        DeleteTimer.Timeout += () =>
        {
            QueueFree();
        };
        AddChild(DeleteTimer);
        AddChild(DamageTimer);
        
    }
    private void DealDamage()
    {
        foreach (var c in DamageChars)
        {
            c.TakeDamage(Damage, Name.ToString().Split('_')[0]);
            c.DamageText(Damage, 1);
        }
    }

    private void DamageAreaEnter(Node2D body)
    {
        if (body is Enemy c)
        {
            DamageChars.Add(c);
        }
    }
    private void DamageAreaExit(Node2D body)
    {
        if (body is Enemy c)
        {
            DamageChars.Remove(c);
        }
    }
}
