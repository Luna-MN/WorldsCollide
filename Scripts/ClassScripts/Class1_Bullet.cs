using Godot;
using System;

public partial class Class1_Bullet : Node2D
{
    public Vector2 MoveDirection;
    
    [ExportGroup("Properties")]
    [Export]
    public int Damage;
    
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        // Move towards the move direction (lerp)
        Position += MoveDirection * (float)delta;
    }
}
