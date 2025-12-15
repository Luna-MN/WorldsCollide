using Godot;
using System;

public partial class Flags : Node
{
    [Flags]
    public enum AbilityFlags : long
    {
        Ranged     = 1L << 0,
        Melee      = 1L << 1,
        Magic      = 1L << 2,
        ChestPiece = 1L << 3,
        Boots      = 1L << 4,
        MainHand     = 1L << 5,
        OffHand      = 1L << 6,
        Helmet       = 1L << 7,
        Gloves       = 1L << 8,
        Legs       = 1L << 9,
        Belt       = 1L << 10,
        TwoHanded   = 1L << 11,
    }
    [Flags]
    public enum PassiveType : long
    {
        None             = 0,
        OnHit            = 1L << 1,
        OnDeath          = 1L << 2,
        OnMove           = 1L << 3,
        OnKill           = 1L << 4,
        StatBoost        = 1L << 5,
        OnTimerTimeout   = 1L << 6,
        DynamicStatBoost = 1L << 7,
        OnCrit           = 1L << 8,
    }

    [Flags]
    public enum Alleigence : long
    {
        Player = 1L << 0,
        Minion = 1L << 1,
        AllEnemies = 1L << 2,
        Fantasy = 1L << 3,
        
    }
}
