using System.Collections.Generic;
using Godot;


public partial class ServerRpc : Node2D
{
    [Export]
    public PackedScene Player;
    
    #region Server RPCs
    // Server -> Get Positions Sent from clients
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void GetPositions(Godot.Collections.Dictionary<int, Vector2> ObjectPositions)
    {
        foreach (var position in ObjectPositions)
        {
            ServerManager.ChangedPosition[position.Key] = position.Value;
        }
    }
    // Server -> Create a player with a given Id
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CreateNewPlayer(int id)
    {
        if(id == GameManager.LocalID) return;
        var newPlayer = Player.Instantiate<Player>();
        newPlayer.Name = id.ToString();
        newPlayer.IsPlayer = false;
        ServerManager.spawner.AddChild(newPlayer, true);
        GD.Print("New player created" + id);
    }
    #endregion
}