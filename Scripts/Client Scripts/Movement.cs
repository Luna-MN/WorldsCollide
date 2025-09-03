using Godot;
using System;

public partial class Movement : Node2D
{
    private Player player;
    [Export] public float Speed = 200f;

    public string playerId;
    private Vector2 lastSentPosition;
    private double sendAccumulator;
    private Timer sendTimer;

    // Networking/throttling settings
    [Export] public double SendIntervalMs = 50; // 20 Hz
    [Export] public float SendDistanceThreshold = 2f;

    public override void _Ready()
    {
        player = GetParent<Player>();
        if (player != null)
        {
            // Prefer a stable ID if you have one; fall back to path string once.
            lastSentPosition = player.GlobalPosition;
        }
        

    }

    public override void _PhysicsProcess(double delta)
    {
        if (player == null || !player.IsPlayer) return;

        // Godot: up is negative Y
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input != Vector2.Zero)
        {
            player.GlobalPosition += input.Normalized() * Speed * (float)delta;
            var pos = player.GlobalPosition;
            if (pos.DistanceSquaredTo(lastSentPosition) < SendDistanceThreshold * SendDistanceThreshold)
                return;

            try
            {
                // This must be non-blocking. If it blocks, it will stall the timer callback (and the main thread).
                GameManager.ChangedPosition(playerId, pos);
                lastSentPosition = pos;
            }
            catch (Exception e)
            {
                GD.PushWarning($"ChangedPosition threw: {e.Message}");
            }
        }
    }
}
