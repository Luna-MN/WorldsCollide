using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ClientRpc : Node2D
{
    [Export]
    public PackedScene FloatingTextScene;
    public TerrainTileMap TerrainTileMap;
    #region Client RPCs

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RemovePlayer(string obj)
    {
        GetNode(obj).QueueFree();
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void AddPlayer(string path)
    {
        GameManager.Players.Add(GetNode<Character>(path));
    }
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 1, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
    public void FloatingText(float value, float multiplier, string nodePath, Color color)
    {
        var floatingText = FloatingTextScene.Instantiate<FloatingText>();
        var node = GetNode(nodePath);
        floatingText.Modulate = color;
        floatingText.value = value;
        floatingText.multiplier = multiplier;
        floatingText.character = (Character)node;
        node.AddChild(floatingText, true);
    }
    //creates tiles on client
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 2, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void ChangeClientTerrainTile(Vector2I coords, int sourceId, Vector2I atlasCoords, int alternativeTile)
    {
        if (TerrainTileMap == null)
        {
            TerrainTileMap = GetTree().Root.GetChildren().FirstOrDefault(x=>x.GetChildren().Count!= 0)?.GetNode<TerrainTileMap>("Background");
        }
        TerrainTileMap?.SetInternalTile(coords, sourceId, atlasCoords, alternativeTile);
    }
    
    //goofy aaah bullshit no Vector2i as Variant so fuck you Vector2 - I hate it too
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferChannel = 2, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void ChangeClientTerrainTiles(Vector2[] v2Coords, int[] sourceId, Vector2[] v2AtlasCoords, int[] alternativeTile)
    {
        if (TerrainTileMap == null)
        {
            TerrainTileMap = GetTree().Root.GetChildren().FirstOrDefault(x=>x.GetChildren().Count!= 0)?.GetNode<TerrainTileMap>("Background");
        }
        Vector2I[] coords = new Vector2I[v2Coords.Length];
        Vector2I[] atlasCoords = new Vector2I[v2AtlasCoords.Length];
        for (int i = 0; i < coords.Length; i++)
        {
            coords[i] = new((int)v2Coords[i].X, (int)v2Coords[i].Y);
            atlasCoords[i] = new ((int)v2AtlasCoords[i].X, (int)v2AtlasCoords[i].Y);
        }
        TerrainTileMap?.SetInternalTiles(coords, sourceId, atlasCoords, alternativeTile);
    }
    #endregion
}
