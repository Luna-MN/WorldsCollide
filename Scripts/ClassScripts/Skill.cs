using Godot;
using System;
[GlobalClass]
public partial class Skill : Resource
{
    public enum RpcLocation
    {
        ClassRpc,
        EquipmentRpc,
        EnemyRpc
    };

    public enum PassiveType
    {
        none,
        OnHit,
        OnDeath,
        OnMove,
    }
    [Export]
    public string Name;
    [Export]
    public bool IsPassive;
    [Export]
    public string RpcName;
    [Export]
    public RpcLocation RpcCallLocation;
    [Export]
    public PassiveType passiveType;
}
