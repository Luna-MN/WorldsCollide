using Godot;
using System;
[GlobalClass]
public partial class Projectile : Node2D
{
    public Vector2 MoveDirection;
    public long Id;
    [Export]
    public Area2D Area;
    
    [ExportGroup("Properties")]
    [Export]
    public int Damage;
    [Export]
    public float Speed = 400f;
    [Export]
    public float RangeTime = 2f;
    
    protected Timer timer;
    
    public override void _Ready()
    {
        // Bullet timeout
        timer = new Timer()
        {
            WaitTime = RangeTime,
            OneShot = true,
            Autostart = true,
            Name = "Timer"
        };
        timer.Timeout += () =>
        {
            if(!Multiplayer.IsServer()) return;
            QueueFree();
        };
        AddChild(timer);
        LookAt(GlobalPosition + MoveDirection);
        
        //On Bullet Hit
        Area.BodyEntered += OnBulletHit;
    }
    
    public override void _EnterTree()
    {
        SetMultiplayerAuthority(1);
    }

    protected virtual void OnBulletHit(Node2D Body)
    {
        if (ServerManager.NodeDictionary[(int)Id] != null && ServerManager.NodeDictionary[(int)Id] is Player bulletOwner)
        {
            bulletOwner.CallOnHit(Body);
        }
    }
}
