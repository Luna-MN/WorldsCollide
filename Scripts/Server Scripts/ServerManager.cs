using Godot;
using System;
using System.Collections.Generic;

public static class ServerManager
{
    public static ServerRpc ServerRpcs;
    public static ClientRpc ClientRpcs;
    public static Godot.Collections.Dictionary<int, Vector2> ChangedPosition = new();
}
