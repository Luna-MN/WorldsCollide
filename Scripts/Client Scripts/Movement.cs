using Godot;
using System;

public partial class Movement : Node2D
{
    private Player player;
    [Export] public float Speed = 200f;

    public int playerId;
    private Vector2 lastSentPosition;
    private double sendAccumulator;
    private Timer sendTimer;

    // Networking/throttling settings
    [Export] public double SendIntervalMs = 50; // 20 Hz
    [Export] public float SendDistanceThreshold = 2f;

    
    [Export] public float Smoothness = 12f;
    [Export] public float SnapDistance = 64f;

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

    }
}
