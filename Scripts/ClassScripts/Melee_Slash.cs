using Godot;
using System;

public partial class Melee_Slash : Gun_Bullet
{
    public Character character;
    
    public override void _PhysicsProcess(double delta)
    {
        if(!Multiplayer.IsServer()) return;
        GlobalPosition = character.GlobalPosition;
    }

    public override void OnBulletHit(Node2D Body)
    {
        
    }
}
