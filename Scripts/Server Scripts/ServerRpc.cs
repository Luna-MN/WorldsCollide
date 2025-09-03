using System.Collections.Generic;
using Godot;


public partial class ServerRpc : Node2D
{
    #region Server RPCs
    // Server -> Get Positions Sent from clients
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.UnreliableOrdered)]
    public void GetPositions(Dictionary<string, Vector2> ObjectPositions)
    {
        foreach (var position in ObjectPositions)
        {
            ServerManager.ChangedPosition[position.Key] = position.Value;
        }
    }
    #endregion
}