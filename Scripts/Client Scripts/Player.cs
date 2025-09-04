using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public long ID;
    [Export] public Movement Movement;
    [Export] public float Speed = 200f;
    public bool IsPlayer { get; set; }
    public override void _Ready()
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }

    public void Move()
    {
        if (!IsPlayer) return;

        // Godot: up is negative Y
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input != Vector2.Zero)
        {
            GlobalPosition += input.Normalized() * Speed;
            var pos = GlobalPosition;
            // if (pos.DistanceSquaredTo(lastSentPosition) < SendDistanceThreshold * SendDistanceThreshold)
            //     return;
            //
            // try
            // {
            //     // This must be non-blocking. If it blocks, it will stall the timer callback (and the main thread).
            //     GameManager.ChangedPosition(playerId, pos);
            //     lastSentPosition = pos;
            // }
            // catch (Exception e)
            // {
            //     GD.PushWarning($"ChangedPosition threw: {e.Message}");
            // }
        }
    }
}
