using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BasicRangedProjectile : Projectile
{
    [Export]
    public bool deleteOnHit = true;
    public int RicochetCount = 0;
    public Area2D RicochetArea;
    public CollisionShape2D RicochetShape;
    public List<Character> charactersIn = [];
    public override void _Ready()
    {
        base._Ready();
        RicochetArea = new Area2D();
        AddChild(RicochetArea);
        RicochetShape = new CollisionShape2D();
        RicochetArea.AddChild(RicochetShape);
        RicochetShape.Shape = new CircleShape2D()
        {
            Radius = 10
        };
        RicochetArea.BodyEntered += RicochetBodyEntered;
        RicochetArea.BodyExited += RicochetBodyExited;
    }

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
            if (RicochetCount > 0)
            {
                var closestPlayer = charactersIn[0];
                foreach (var character in charactersIn)
                {
                    if (closestPlayer.GlobalPosition.DistanceTo(GlobalPosition) > character.GlobalPosition.DistanceTo(GlobalPosition))
                    {
                        closestPlayer = character;    
                    }
                }
                MoveDirection = closestPlayer.GlobalPosition;
                RicochetCount--;
            }
            else if (deleteOnHit)
            {
                QueueFree();
            }
        }
    }

    public void RicochetBodyEntered(Node2D Body)
    {
        if (Body is Character c && c.alleigence.HasFlag(Flags.Alleigence.AllEnemies))
        {
            charactersIn.Add(c);
        }
    }
    public void RicochetBodyExited(Node2D Body)
    {
        if (Body is Character c && c.alleigence.HasFlag(Flags.Alleigence.AllEnemies))
        {
            charactersIn.Remove(c);
        }
    }
}
