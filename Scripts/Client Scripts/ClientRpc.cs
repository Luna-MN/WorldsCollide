using Godot;
using System;
using System.Collections.Generic;

public partial class ClientRpc : Node2D
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
    // Client -> Send positions that have changed since the last call
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void SendPositions(Dictionary<string, Vector2> ObjectPositionChanges)
    {
        foreach (var change in ObjectPositionChanges)
        {
            GameManager.ObjectPositions[change.Key] = change.Value;
        }
    }
    #endregion
}
