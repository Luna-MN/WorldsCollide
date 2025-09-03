using Godot;
using System;
using System.Collections.Generic;

public partial class TestScene : Node2D
{
    private Timer SendPositionsTimer;
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
            GD.Print(GameManager.ObjectPositions.Count);
            var obj = GetNode<Player>(change.Key);
            if (obj != null && obj.GlobalPosition != change.Value)
            {
                obj.GlobalPosition = change.Value;
            }
            else if (obj == null)
            {
                GD.Print("Object not found" + change.Key);
            }
        }
    }

    private void TestStuff()
    {
        GetNode<Player>("Player").IsPlayer = true;
        GetNode<Player>("Player").Name = "Player " + Multiplayer.GetUniqueId();
        
        SendPositionsTimer.Start();
    }


}
