using Godot;
using System;
[GlobalClass]
public partial class Gun_Bullet : Projectile
{

    public override void _PhysicsProcess(double delta)
    {
        if(!Multiplayer.IsServer()) return;
        // Move towards the move direction
        float length = MoveDirection.Length();
        if (length > 0)
        {
            GlobalPosition += MoveDirection * ((float)delta * Speed / length);
        }
    }
    protected override void OnBulletHit(Node2D Body)
    {
        if(!Multiplayer.IsServer()) return;
        if (Body is Player p)
        {
            if(p.Name == Id.ToString()) return;
        }

        base.OnBulletHit(Body);
        QueueFree();
        
    }
}
