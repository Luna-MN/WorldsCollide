using System.Collections.Generic;
using Godot;


public partial class ServerRpc : Node2D
{
    [Export]
    public PackedScene Player;
    
    #region Server RPCs
    // Server -> Create a player with a given Id
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CreateNewPlayer(int id)
    {
        if(id == GameManager.LocalID) return;
        var newPlayer = Player.Instantiate<Player>();
        newPlayer.Name = id.ToString();
        newPlayer.IsPlayer = false;
        ServerManager.spawner.AddChild(newPlayer, true);
        ServerManager.NodeDictionary.Add(id, newPlayer);
        GD.Print("New player created" + id);
    }
    #endregion
}