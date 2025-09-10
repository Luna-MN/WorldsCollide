using Godot;
using System;
[GlobalClass]
public partial class Skill : Resource
{
    [Export]
    public string Name;
    [Export]
    public Texture2D Icon;

    public enum PassiveType
    {
        None,
        OnHit,
        OnDeath,
        OnMove,
        OnKill,
        StatBoost,
        OnTimerTimeout,
        DynamicStatBoost
    }
    [ExportGroup("Passives")]
    [Export]
    public bool IsPassive;
    [Export]
    public PassiveType passiveType;
    [Export(PropertyHint.None, "This is for the OnTimerTimeout and OnMove Passives, Time in seconds until the thing happens")]
    public float TimerWaitTime = 2f;
    [Export(PropertyHint.None, "This is the stat that will be modified by the Passive (StatBoost only)")]
    public string PassiveStat;
    [Export(PropertyHint.None, "This is mainly for the StatBoost Passive, the value to add to the stat")]
    public float PassiveValue;
    [ExportGroup("RPC")]
    [Export]
    public string RpcName;
    
    [ExportGroup("Stats")]
    [Export]
    public float Damage = 10f;
    [Export]
    public float RangeTime = 1f;
    
    
    public enum RpcLocation
    {
        ClassRpc,
        EquipmentRpc,
        EnemyRpc
    };
    [Export]
    public RpcLocation RpcCallLocation;

}
