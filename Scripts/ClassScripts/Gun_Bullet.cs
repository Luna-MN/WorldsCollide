using Godot;
using System;

public partial class Gun_Bullet : Node2D
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
    
    private Timer timer;


    public override void _EnterTree()
    {
        SetMultiplayerAuthority(1);
    }
    public override void _Ready()
    {
        // Bullet timeout
        Timer timer = new Timer()
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
    public virtual void OnBulletHit(Node2D Body)
    {
        if(!Multiplayer.IsServer()) return;
        if (Body is Player p)
        {
            if(p.Name == Id.ToString()) return;
        }
        QueueFree();
    }
}
