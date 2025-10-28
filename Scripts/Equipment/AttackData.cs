using Godot;
using System;

[GlobalClass]
public partial class AttackData : Resource
{

    [Export] public string Animation { get; set; } = "attack";
    [Export] public Stats stats;
    [Export] public PrimaryWeapon.WeaponType type;
    [Export] public float RicochetCount;
    public Timer attackTimer;
    public bool Available = true;
    public bool Event;
}
