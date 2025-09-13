using Godot;
using System;
[GlobalClass]
public partial class BasicRangedProjectile : Projectile
{
    [Export]
    bool deleteOnHit = true;
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
    protected override async void OnBulletHit(Node2D Body)
    {
        base.OnBulletHit(Body);
        if(Name.ToString().Contains(Body.Name.ToString())) return;
        if (Multiplayer.IsServer())
        {
            var timer = new Timer()
            {
                Autostart = true,
                OneShot = true,
                WaitTime = 0.05f
            };
            AddChild(timer);
            timer.Timeout += () =>
            {
                timer.QueueFree();
            };
            await ToSignal(timer, "timeout");
            if(deleteOnHit)
                QueueFree();
        }

    }
}
