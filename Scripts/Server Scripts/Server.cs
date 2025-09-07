using Godot;
using System;
using System.Collections.Generic;

public partial class Server : Node2D
{
    private ENetMultiplayerPeer Network;
    [Export]
    public int Port = 7777;
    [Export]
    public PackedScene Dummy;
    private List<long> PlayerIds = new List<long>();
    public Server()
    {
        CreateServer();
    }
    private void CreateServer()
    {
        Network = new ENetMultiplayerPeer();
        var err = Network.CreateServer(Port);
        if (err != Error.Ok)
        {
            // Make the failure explicit
            throw new InvalidOperationException($"Failed to create server on port {Port}: {err}");
        }
    }

    private void CreateTrainingDummy()
    {
        var dummy = Dummy.Instantiate<Character>();
        dummy.Name = "Dummy";
        dummy.IsDummy = true;
        dummy.GlobalPosition = new Vector2(300, 200);
        dummy.SetMultiplayerAuthority(1);
        ServerManager.spawner.AddChild(dummy, true);
    }
    public override void _Ready()
    {
        ServerManager.ServerRpcs = GetNode<ServerRpc>("/root/ServerRpc");
        ServerManager.ClientRpcs = GetNode<ClientRpc>("/root/ClientRpc");
        ServerManager.ClassRpcs = GetNode<ClassRpc>("/root/ClassRpc");
        ServerManager.EquipmentRpcs = GetNode<EquipmentRpc>("/root/EquipmentRpc");
        ServerManager.spawner = GetParent().GetNode<MultiplayerSpawner>("MultiplayerSpawner");
        Multiplayer.MultiplayerPeer = Network;
    }

    
}