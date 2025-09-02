using Godot;
using System;

public partial class Rpc : Node2D
{
    [Export]
    public PackedScene Player;
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CreateNewPlayer(long id)
    {
        if(id == GameManager.LocalID) return;
        var newPlayer = Player.Instantiate<Player>();
        newPlayer.Name = "Player " + id;
        newPlayer.IsPlayer = false;
        AddChild(newPlayer);
    }
}
