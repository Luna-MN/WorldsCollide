using Godot;
using System;

public partial class Camera : Camera2D
{
    private Node2D player;
    private bool found;
    public override void _Ready()
    {
        if (Multiplayer.IsServer())
            return;

        TryResolvePlayer();

    }
    public override void _Process(double delta)
    {
        if (Multiplayer.IsServer())
            return;

        if (player == null || !GodotObject.IsInstanceValid(player))
            TryResolvePlayer();

        if (player != null)
            GlobalPosition = player.GlobalPosition;

    }
    private void TryResolvePlayer()
    {
        // Prefer the active scene root; fall back to the tree root if needed
        var sceneRoot = GetTree().CurrentScene ?? GetTree().Root;
        if (sceneRoot == null)
            return;

        // 1) Try under MultiplayerSpawner directly under the scene root
        var spawner = sceneRoot.GetNodeOrNull<Node>("MultiplayerSpawner");
        if (spawner != null)
        {
            player = spawner.GetNodeOrNull<Node2D>(Multiplayer.GetUniqueId().ToString());
            if (player != null)
                return;
        }

        // 2) Fallback: deep search anywhere under the scene root
        player = sceneRoot.FindChild(Multiplayer.GetUniqueId().ToString(), recursive: true, owned: false) as Node2D;
    }

}
