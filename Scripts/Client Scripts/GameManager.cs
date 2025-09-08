using Godot;
using System;
using System.Collections.Generic;
public static class GameManager
{
    public static List<Player> Players;
    public static long LocalID;
    public static ServerRpc ServerRpcs;
    public static ClientRpc ClientRpcs;
    public static ClassRpc ClassRpcs;
    public static EquipmentRpc EquipmentRpcs;
    public static Camera2D camera;
    public static Player player;
}
