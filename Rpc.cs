using Godot;
using System;
using System.Collections.Generic;

public partial class Rpc : Node2D
{
    [Export]
    public PackedScene Player;
    
    #region Client RPCs
    // Client -> Create a player with a given Id
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void CreateNewPlayer(long id)
    {
        if(id == GameManager.LocalID) return;
        var newPlayer = Player.Instantiate<Player>();
        newPlayer.Name = "Player " + id;
        newPlayer.IsPlayer = false;
        AddChild(newPlayer);
    }
    // Client -> Get positions that have changed since the last call
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void GetVectors(Dictionary<string, Vector2> ObjectPositionChanges)
    {
        foreach (var change in ObjectPositionChanges)
        {
            GameManager.ObjectPositions[change.Key] = change.Value;
        }
    }
    #endregion
    
    #region Server RPCs
    // Server -> Get Positions Sent from clients
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void SendPositions(Dictionary<string, Vector2> ObjectPositions)
    {
        
    }
    #endregion
}
