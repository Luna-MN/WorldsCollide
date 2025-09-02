using Godot;
using System;

public partial class ServerManagmentNode : Node
{
    private Timer sendPositionsTimer = new();
    private Server server;
    public override void _Ready()
    {
        server = GetNode<Server>("/root/Server");
        sendPositionsTimer = new Timer()
        {
            Autostart = true,
            OneShot = false,
            WaitTime = 0.05f,
            Name = "Position Timer"
        };
        sendPositionsTimer.Timeout += SendPositions;
        AddChild(sendPositionsTimer);
    }
    private void SendPositions()
    {
        server.Rpcs.Rpc("SendPositions", ServerManager.ChangedPosition);
    }
}
