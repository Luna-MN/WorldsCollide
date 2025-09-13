using Godot;
using System;

public partial class Circling_Projectile : Projectile
{
    [Export] public float distance = 75;
    [Export] public float speed = 1;
    public Character character;
    public double angle;
    
    
    public override void _PhysicsProcess(double delta)
    {
        if(!Multiplayer.IsServer()) return;
        angle += (2 * double.Pi * speed  * delta);

        GlobalPosition = character.GlobalPosition + Vector2.FromAngle((float)angle) * distance;

    }
}
