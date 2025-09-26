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
    public void FloatingText(string text, string nodePath, Color color)
    {
        var floatingText = FloatingTextScene.Instantiate<FloatingText>();
        var node = GetNode(nodePath);
        floatingText.Modulate = color;
        floatingText.text.Text = text;
        node.AddChild(floatingText, true);
    }
    #endregion
}
