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
    // if its a passive it won't evaluate when you click the skill button and the passive will be applied when you set the skill
    [Export(PropertyHint.GroupEnable)]
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
    // this name is what RPC function gets called (classname)_(RpcName)
    [Export]
    public string RpcName;
    [Export]
    public RpcLocation RpcCallLocation;
    [ExportGroup("Stats")]
    [Export]
    public float Damage = 10f;
    [Export]
    public float RangeTime = 1f;

    [ExportGroup("Enemy")] 
    [Export(PropertyHint.GroupEnable)] public bool IsEnemy;
    public int ExecuteValue;
    public virtual void EnemySkillEvealuation()
    {
        // in here we can set the ExecuteValue based on what the enemy is doing and then execute the skill based on that 
    }
    
    public enum RpcLocation
    {
        ClassRpc,
        EquipmentRpc,
        EnemyRpc
    };


}
