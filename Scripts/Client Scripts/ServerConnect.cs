using Godot;
using System;

public partial class ServerConnect : Node2D
{
    [Export]
    public PackedScene Player;
    private ENetMultiplayerPeer Server;
    
    public override void _Ready()
    {
        GameManager.ServerRpcs = GetNode<ServerRpc>("/root/ServerRpc");
        GameManager.ClientRpcs = GetNode<ClientRpc>("/root/ClientRpc");
        GameManager.ClassRpcs = GetNode<ClassRpc>("/root/ClassRpc");
        GameManager.EquipmentRpcs = GetNode<EquipmentRpc>("/root/EquipmentRpc");
        Connect();
    }
    private async void Connect()
    {
        Server = new ENetMultiplayerPeer();
        Server.CreateClient("127.0.0.1", 7777);
        Multiplayer.MultiplayerPeer = Server;
        Multiplayer.ConnectedToServer += ConnectionSuccess;
        Multiplayer.ConnectionFailed += ConnectionFailed;
    }

    private void ConnectionSuccess()
    {
        GD.Print("Connected");
        GameManager.LocalID = Multiplayer.GetUniqueId();
    }
    private void ConnectionFailed()
    {
        GD.Print("Failed");
    }
}
