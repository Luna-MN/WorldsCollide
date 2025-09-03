using System.Collections.Generic;
using Godot;


public partial class ServerRpc : Node2D
{
    #region Server RPCs
    // Server -> Get Positions Sent from clients
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void GetPositions(Godot.Collections.Dictionary<string, Vector2> ObjectPositions)
    {
        foreach (var position in ObjectPositions)
        {
            ServerManager.ChangedPosition[position.Key] = position.Value;
        }
    }
    #endregion
}