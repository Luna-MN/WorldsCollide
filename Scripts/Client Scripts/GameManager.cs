using Godot;
using System;
using System.Collections.Generic;
public static class GameManager
{
    public static List<Character> Players = [];
    public static string LocalID;
    public static ServerRpc ServerRpcs;
    public static ClientRpc ClientRpcs;
    public static ClassRpc ClassRpcs;
    public static EquipmentRpc EquipmentRpcs;
    public static Camera2D camera;
    public static Character player;
    public static bool UIOpen;
    public static Enhancments Enhancments;
    public static EquipmentSlots EquipmentSlots;
    public static UiController UiController;
}
