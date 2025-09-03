using Godot;
using System;
using System.Collections.Generic;
public static class GameManager
{
    public static List<Player> Players;
    public static long LocalID;
    public static ServerRpc ServerRpcs;
    public static ClientRpc ClientRpcs;
    public static Dictionary<string, Vector2> ObjectPositions = new Dictionary<string, Vector2>();
    public static Godot.Collections.Dictionary<string, Vector2> ChangedPositions = new();
    public static void ChangedPosition(string name, Vector2 position)
    {
        if (ObjectPositions.TryGetValue(name, out var localPos))
        {
            if (localPos != position)
            {
                ChangedPositions[name] = position;
                ObjectPositions[name] = position;
                return;
            }
        }
        ObjectPositions[name] = position;
        ChangedPositions[name] = position;
    }
}
