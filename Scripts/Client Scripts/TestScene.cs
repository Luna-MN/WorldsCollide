using Godot;
using System;
using System.Collections.Generic;

public partial class TestScene : Node2D
{
    private Timer SendPositionsTimer;
    
    // Smoothing config
    private const float SmoothingSpeed = 1f;   // Higher = faster convergence
    private const float SnapDistance   = 200f;  // Snap instead of lerp if correction is very large
    private Node2D Server;
    public override void _EnterTree()
    {
        try
        {
            Server = new Server();
            GD.Print("I am server");
        }
        catch (Exception e)
        {
            GD.PushWarning($"Failed to create server: {e.Message}");
            Server = new ServerConnect();
            GD.Print("I am client");
        }
    }

    public override void _Ready()
    {
        AddChild(Server);
        if (Server is ServerConnect)
        {
            Multiplayer.ConnectedToServer += TestStuff;
            GameManager.camera = GetNode<Camera2D>("Camera2D");
        }

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


    }

    private void TestStuff()
    {
        GameManager.ServerRpcs.RpcId(1, "CreateNewPlayer", Multiplayer.GetUniqueId());
    }


}
