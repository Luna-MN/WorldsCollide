using Godot;
using System;
using System.Collections.Generic;

public static class ServerManager
{
    public static ServerRpc ServerRpcs;
    public static ClientRpc ClientRpcs;
    public static ClassRpc ClassRpcs;
    public static MultiplayerSpawner spawner;
    public static Dictionary<int, Character> NodeDictionary = new Dictionary<int, Character>();
}
