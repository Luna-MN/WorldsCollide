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
        Node2D Parent;
        var parents = GetTree().Root.GetChildren();
        foreach (var parent in parents)
        {
            if (parent.Name.ToString().Contains("Rpc"))
            {
                continue;
            }
            parent.AddChild(newPlayer);
            break;
        }
    }
    // Client -> Send positions that have changed since the last call
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void SendPositions(Godot.Collections.Dictionary<string, Vector2> ObjectPositionChanges)
    {
        foreach (var change in ObjectPositionChanges)
        {
            GameManager.ObjectPositions[change.Key] = change.Value;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void RequestPositions()
    {
        GameManager.ServerRpcs.RpcId(1, "GetPositions", GameManager.ChangedPositions);
        GameManager.ChangedPositions.Clear();
    }
    #endregion
}
