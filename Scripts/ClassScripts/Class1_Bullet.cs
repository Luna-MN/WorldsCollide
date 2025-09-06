using Godot;
using System;

public partial class Class1_Bullet : Node2D
{
    public Vector2 MoveDirection;
    public long Id;
    
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
        Timer timer = new Timer()
        {
            WaitTime = RangeTime,
            OneShot = true,
            Autostart = true,
            Name = "Timer"
        };
        timer.Timeout += () =>
        {
            QueueFree();
        };
        AddChild(timer);
        LookAt(GlobalPosition + MoveDirection);
    }
    public override void _PhysicsProcess(double delta)
    {
        if(!Multiplayer.IsServer()) return;
        // Move towards the move direction
        float length = MoveDirection.Length();
        if (length > 0)
        {
            Position += MoveDirection * ((float)delta * Speed / length);
        }

    }
}
