using Godot;
using System;
using System.Collections.Generic;

public partial class Server : Node
{
    private ENetMultiplayerPeer Network;
    [Export]
    public int Port = 7777;

    private Rpc Rpcs;
    private List<long> PlayerIds = new List<long>();
    public override void _Ready()
    {
        CreateServer();
        Rpcs = GetNode<Rpc>("/root/Rpc");
    }
    private void CreateServer()
    {
        Network = new ENetMultiplayerPeer();
        Network.CreateServer(Port);
        
        if (Network.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected)
        {
            GD.Print("Failed to create server");
        }
        else
        {
            GD.Print("Server created");
        }
        
        Multiplayer.MultiplayerPeer = Network;
        Network.PeerConnected += OnPeerConnected;

    }

    private void OnPeerConnected(long id)
    {
        GD.Print("Peer connected");


        GD.Print("Creating player");
        Rpcs.Rpc("CreateNewPlayer", id);
        foreach (var oldIds in PlayerIds)
        {
            Rpcs.RpcId(id, "CreateNewPlayer", oldIds);
        }
        PlayerIds.Add(id);
    }
}