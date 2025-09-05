using Godot;
using System;
using System.Collections.Generic;
public static class GameManager
{
    public static List<Player> Players;
    public static long LocalID;
    public static ServerRpc ServerRpcs;
    public static ClientRpc ClientRpcs;
    public static Dictionary<int, Vector2> ObjectPositions = new Dictionary<int, Vector2>();
    public static Godot.Collections.Dictionary<int, Vector2> ChangedPositions = new();
    
    public static Dictionary<int, Node2D> NodeDictionary = new Dictionary<int, Node2D>();
    
    public static Camera2D camera;
    public static void ChangedPosition(int id, Vector2 position)
    {
        if (ObjectPositions.TryGetValue(id, out var localPos))
        {
            if (localPos != position)
            {
                ChangedPositions[id] = position;
                ObjectPositions[id] = position;
                return;
            }
        }
        ObjectPositions[id] = position;
        ChangedPositions[id] = position;
    }
}
