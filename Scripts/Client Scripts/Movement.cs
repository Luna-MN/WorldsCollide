using Godot;
using System;

public partial class Movement : Node2D
{
    private Player player;
    [Export] public float Speed = 200f;

    public override void _Ready()
    {
        player = GetParent<Player>();
    }

    public override void _Process(double delta)
    {
        if (player == null || !player.IsPlayer) return;

        // Godot: up is negative Y
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (input != Vector2.Zero)
        {
            player.GlobalPosition += input.Normalized() * Speed * (float)delta;
            GameManager.ChangedPosition(player.GetPath().ToString(), player.GlobalPosition);
        }
    }
}
