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
}
