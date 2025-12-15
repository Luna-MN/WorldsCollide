using Godot;
using System;

public partial class Camera : Camera2D
{
    private Character player;
    private bool found;
    [Export]
    public bool followPlayer = true;
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
        {
            if (followPlayer)
            {
                GlobalPosition = player.GlobalPosition;
            }
            else
            {
                var playerPos = player.GlobalPosition;
                var dir = (player.TargetPosition - player.GlobalPosition).Normalized();
                var target = Vector2.Zero;
                if (playerPos.DistanceTo(player.TargetPosition) > 100)
                {
                    target = playerPos + dir *  50;
                }
                else
                {
                    target = (playerPos + player.TargetPosition) /2;
                }

            
                GlobalPosition = target;
            }
        }

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
            player = spawner.GetNodeOrNull<Character>(Multiplayer.GetUniqueId().ToString());
            if (player != null)
                return;
        }

        // 2) Fallback: deep search anywhere under the scene root
        player = sceneRoot.FindChild(Multiplayer.GetUniqueId().ToString(), recursive: true, owned: false) as Character;
    }

}
