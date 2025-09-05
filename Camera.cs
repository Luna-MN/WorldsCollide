using Godot;
using System;

public partial class Camera : Camera2D
{
    private Node2D player;
    private bool found;
    public override void _Ready()
    {
        player = GetParent().GetNode<Node2D>($"MultiplayerSpawner/{Multiplayer.GetUniqueId()}");
        if (player != null)
        {
            found = true;
        }
    }
    public override void _Process(double delta)
    {
        if(Multiplayer.IsServer()) return;
        if (!found)
        {
            player = GetParent().GetNode<Node2D>($"MultiplayerSpawner/{Multiplayer.GetUniqueId()}");
            if (player != null)
            {
                found = true;
            }
        }
        if (found)
        {
            GlobalPosition = player.GlobalPosition;
        }

    }
}
