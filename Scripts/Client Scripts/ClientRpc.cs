using Godot;
using System;
using System.Collections.Generic;

public partial class ClientRpc : Node2D
{

    
    #region Client RPCs

    // Client -> Send positions that have changed since the last call
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void SendPositions(Godot.Collections.Dictionary<int, Vector2> ObjectPositionChanges)
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
