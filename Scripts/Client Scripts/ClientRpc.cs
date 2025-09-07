using Godot;
using System;
using System.Collections.Generic;

public partial class ClientRpc : Node2D
{
    
    #region Client RPCs

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RemovePlayer(string obj)
    {
        GetNode(obj).QueueFree();
    }
    
    #endregion
}
