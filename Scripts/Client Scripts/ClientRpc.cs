using Godot;
using System;
using System.Collections.Generic;

public partial class ClientRpc : Node2D
{
    [Export]
    public PackedScene FloatingTextScene;
    #region Client RPCs

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RemovePlayer(string obj)
    {
        GetNode(obj).QueueFree();
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void FloatingText(float value, float multiplier, string nodePath, Color color)
    {
        var floatingText = FloatingTextScene.Instantiate<FloatingText>();
        var node = GetNode(nodePath);
        floatingText.Modulate = color;
        floatingText.value = value;
        floatingText.multiplier = multiplier;
        node.AddChild(floatingText, true);
    }
    #endregion
}
