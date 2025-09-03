using Godot;
using System;
using System.Collections.Generic;

public partial class TestScene : Node2D
{
    private Timer SendPositionsTimer;
    
    // Smoothing config
    private const float SmoothingSpeed = 1f;   // Higher = faster convergence
    private const float SnapDistance   = 200f;  // Snap instead of lerp if correction is very large

    public override void _Ready()
    {
        Multiplayer.ConnectedToServer += TestStuff;
        SendPositionsTimer = new Timer()
        {
            Autostart = false,
            OneShot = false,
            WaitTime = 0.05f,
            Name = "Position Timer"
        };
        SendPositionsTimer.Timeout += () =>
        {
            if (GameManager.ChangedPositions.Count == 0)
            {
                return;
            }
            if (Multiplayer.MultiplayerPeer.GetConnectionStatus() != MultiplayerPeer.ConnectionStatus.Connected)
            {
                GD.Print("Not connected");
                SendPositionsTimer.Start();
                return;
            }
            GameManager.ServerRpcs.RpcId(1, "GetPositions", GameManager.ChangedPositions);
            GameManager.ChangedPositions.Clear();
            SendPositionsTimer.Start();
        };
        AddChild(SendPositionsTimer);
    }
    
    public override void _Process(double delta)
    {
        foreach (var change in GameManager.ObjectPositions)
        {
            var obj = GetNode<Player>(change.Key);
            if (obj != null && obj.GlobalPosition != change.Value)
            {
                obj.GlobalPosition = change.Value;
            }
            else if (obj == null)
            {
                GD.Print("Object not found" + change.Key);
            }
            
            if (obj.IsPlayer)
                continue;

            
            Vector2 current = obj.GlobalPosition;
            Vector2 target  = change.Value;

            
            // Large corrections: snap (teleports or big desyncs)
            float distance = current.DistanceTo(target);
            if (distance > SnapDistance)
            {
                obj.GlobalPosition = target;
                continue;
            }

            // Smoothly approach target using exponential smoothing
            float weight = 1f - Mathf.Exp(-SmoothingSpeed * (float)delta); // stable across frame rates
            obj.GlobalPosition = current.Lerp(target, weight);
        }
    }

    private void TestStuff()
    {
        var player = GetNode<Player>("Player");
        player.IsPlayer = true;
        player.Name = "Player " + Multiplayer.GetUniqueId();
        player.Movement.playerId = player.Name.ToString();
        SendPositionsTimer.Start();
    }


}
