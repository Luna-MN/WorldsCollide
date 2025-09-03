using Godot;
using System;

public partial class ServerManagmentNode : Node
{
    private Timer sendPositionsTimer = new();
    public override void _Ready()
    {
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
        ServerManager.ClientRpcs.Rpc("SendPositions", ServerManager.ChangedPosition);
    }
}
