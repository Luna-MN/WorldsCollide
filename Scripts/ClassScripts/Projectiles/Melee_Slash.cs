using Godot;
using System;

public partial class Melee_Slash : Projectile
{
    public Character character;
    
    public override void _PhysicsProcess(double delta)
    {
        if(!Multiplayer.IsServer()) return;
        GlobalPosition = character.GlobalPosition;
    }
    
}
