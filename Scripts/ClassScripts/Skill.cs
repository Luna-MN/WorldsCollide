using Godot;
using System;
[GlobalClass]
public partial class Skill : Resource
{
    [Export]
    public string Name;


    public enum PassiveType
    {
        None,
        OnHit,
        OnDeath,
        OnMove,
        OnKill,
        StatBoost,
        OnTimerTimeout
    }
    [ExportGroup("Passives")]
    [Export]
    public bool IsPassive;
    [Export]
    public PassiveType passiveType;
    [Export]
    public float TimerWaitTime = 2f;
    [Export]
    public float PassiveValue;
    
    [ExportGroup("RPC")]
    [Export]
    public string RpcName;
    public enum RpcLocation
    {
        ClassRpc,
        EquipmentRpc,
        EnemyRpc
    };
    [Export]
    public RpcLocation RpcCallLocation;

}
