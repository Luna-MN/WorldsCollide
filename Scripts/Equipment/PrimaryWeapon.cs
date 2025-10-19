using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class PrimaryWeapon : BaseEquipment
{
    public enum WeaponType
    {
        None,
        Projectile,
        Slash,
        Beam,
        Charge
        // Add Others Here
    }

    public enum EventAction
    {
        click,
        unclick,
        fire,
        
    }
    [Export] public AttackData[] Attacks;
    [Export] public string WeaponName;
    [Export] public string Description;
    [Export] public bool HasRightClick;
    [Export] public Vector2 ShootPos;
    [Export] public Vector2 FlippedPos;
    [Export] public SpriteFrames SpriteFrames;
    [Export] public Stats[] Stats;
    public bool TwoHandedMode;
    public override void OnEquip(Character character)
    {
        if (Attacks.Length < 0)
        {
            return;
        }
        

        foreach (var Data in Attacks)
        {
            Data.attackTimer = CreateTimer(Data);
            if (Data.attackTimer != null)
            {
                Data.attackTimer.Timeout += () => { Data.Available = true; };
                character.AddChild(Data.attackTimer);
                if (Reset(Data.type))
                {
                    Data.attackTimer.Start();
                }
            }
        }
        
        if (equipmentFlags.HasFlag(Flags.AbilityFlags.TwoHanded) && TwoHandedMode)
        {
            GD.Print("Is Two Handed");
            character.PrimaryEquipment.Add(this);
            character.SecondaryEquipment.Add(this);
        }
        else if (equipmentFlags.HasFlag(Flags.AbilityFlags.MainHand) && character.EquipmentSlots.ToList().Find(x => x.EquippedEquipment == this).equipmentFlags.HasFlag(Flags.AbilityFlags.MainHand))
        {
            character.PrimaryEquipment.Add(this);
        }
        else if(equipmentFlags.HasFlag(Flags.AbilityFlags.OffHand) && character.EquipmentSlots.ToList().Find(x => x.EquippedEquipment == this).equipmentFlags.HasFlag(Flags.AbilityFlags.OffHand))
        {
            character.SecondaryEquipment.Add(this);
        }
        base.OnEquip(character);
    }

    public void Left_Click()
    {
        GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_LeftClick", (int)GameManager.LocalID, ItemId);
    }

    public void Right_Click()
    {
        if (!HasRightClick)
        {
            GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_LeftClick", (int)GameManager.LocalID, ItemId, 1);
        }
        else
        {
            GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_RightClick", (int)GameManager.LocalID, ItemId);
        }
    }
    // this decides weather the attack available flag should be reset when the gun is fired 
    public bool Reset(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Projectile:
            case WeaponType.Slash:
                return false;
            case WeaponType.Beam:
                return true;
            default:
                return false;
        }
    }
    // some weps we don't want to have a timer and want to be attack-available on other conditions
    public Timer CreateTimer(AttackData attack)
    {
        if (attack.type == WeaponType.Beam)
        {
            return null;
        }
        var time = attack.stats["AttackSpeed"];
        if (attack.attackTimer != null)
        {
            attack.attackTimer.QueueFree();
            attack.attackTimer = null;
        }
        return new Timer()
        {
            Autostart = false,
            OneShot = true,
            WaitTime = time,
            Name = "Attack Timer"
        };
    }
    // This gets called when you fire
    public bool StartTimerOnReset(WeaponType type)
    {
        if (type == WeaponType.Charge)
        {
            return false;
        }
        return true;
    }
    // this gets called when you click
    public bool StartTimerOnClick(WeaponType type)
    {
        if (type == WeaponType.Charge)
        {
            return true;
        }
        return false;
    }

    public bool Attack(Player c, bool primary, AttackData attack, EventAction action)
    {
        GD.Print(attack.type + " " + Attacks.ToList().IndexOf(attack));
        ref var timer = ref attack.attackTimer;
        ref var Event = ref attack.Event;
        ref var AttackAvailable = ref attack.Available;
        switch (attack.type)
        {
            case WeaponType.Projectile:
                if (action == EventAction.click)
                {
                    Event = true;
                }
                else if (action == EventAction.unclick)
                {
                    Event = false;
                }
                else if (action == EventAction.fire)
                {
                    AttackAvailable = false;
                    GD.Print("Attacking");
                    timer?.Start();
                    return true;
                }

                break;
            case WeaponType.Slash:
                if (action == EventAction.click)           
                {                                          
                    Event = true;                          
                }                                          
                else if (action == EventAction.unclick)    
                {                                          
                    Event = false;                         
                }                                          
                else if (action == EventAction.fire)       
                {                                          
                    AttackAvailable = false;               
                    timer?.Start();                        
                    return true;                           
                }                                          
                break;
            case WeaponType.Beam:
                if (action == EventAction.click)
                {
                    AttackAvailable = true;
                    Event = true;
                }
                if (action == EventAction.unclick)
                {
                    AttackAvailable = false;
                    Event = false;
                }
                else if (action == EventAction.fire)
                {
                    return true;
                }
                break;
            case WeaponType.Charge:
                if (action == EventAction.click)
                {
                    Event = false;
                    timer?.Start();
                }
                else if (action == EventAction.unclick)
                {
                    if (timer.IsStopped())
                    {
                        return true;
                    }
                    timer?.Stop();
                }
                break;
            default:
                if (action == EventAction.click)
                {
                    Event = true;
                }
                else if (action == EventAction.unclick)
                {
                    Event = false;
                }
                else if (action == EventAction.fire)
                {
                    AttackAvailable = false;
                    timer?.Start();
                    return true;
                }
                break;
        }
        return false;
    }
}
