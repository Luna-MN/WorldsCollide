using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class PrimaryWeapon : BaseEquipment
{
    public enum WepeonType
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
    [Export] public string WeaponName;
    [Export] public string Description;
    [Export] public bool HasRightClick;
    [Export] public int Damage;
    [Export] public int Range;
    [Export] public float AttackSpeed;
    [Export] public Vector2 ShootPos;
    [Export] public Vector2 FlippedPos;
    [Export] public SpriteFrames SpriteFrames;
    [Export] public WepeonType LeftType;
    [Export] public WepeonType RightType;
    public bool TwoHandedMode;
    public override void OnEquip(Character character)
    {
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
        GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_LeftClick", (int)GameManager.LocalID);
    }

    public void Right_Click()
    {
        if (!HasRightClick)
        {
            GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_LeftClick", (int)GameManager.LocalID, 1);
        }
        else
        {
            GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_RightClick", (int)GameManager.LocalID);
        }
    }
    // this decides weather the attack available flag should be reset when the gun is fired 
    public bool Reset(WepeonType type)
    {
        switch (type)
        {
            case WepeonType.Projectile:
            case WepeonType.Slash:
                return false;
            case WepeonType.Beam:
                return true;
            default:
                return false;
        }
    }
    // some weps we don't want to have a timer and want to be attack-available on other conditions
    public Timer CreateTimer(WepeonType type)
    {
        if (type == WepeonType.Beam)
        {
            return null;
        }

        return new Timer()
        {
            Autostart = false,
            OneShot = true,
            WaitTime = AttackSpeed,
            Name = "Attack Timer"
        };
    }
    // This gets called when you fire
    public bool StartTimerOnReset(WepeonType type)
    {
        if (type == WepeonType.Charge)
        {
            return false;
        }
        return true;
    }
    // this gets called when you click
    public bool StartTimerOnClick(WepeonType type)
    {
        if (type == WepeonType.Charge)
        {
            return true;
        }
        return false;
    }

    public bool Attack(Player c, bool primary, WepeonType type, EventAction action)
    {
        ref Timer timer = ref c.attack1Timer;
        ref bool AttackAvailable = ref c.Attack1Available; 
        ref bool Event = ref c.LeftEvent;
        if (primary)
        {
            timer = ref c.attack1Timer;
            AttackAvailable = ref c.Attack1Available; 
            Event = ref c.LeftEvent;
        }
        else
        {
            timer = ref c.attack2Timer;
            AttackAvailable = ref c.Attack2Available; 
            Event = ref c.RightEvent;
        }
        switch (type)
        {
            case WepeonType.Projectile:
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
            case WepeonType.Slash:
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
            case WepeonType.Beam:
                if (action == EventAction.click)
                {
                    AttackAvailable = true;
                    Event = true;
                }
                else if (action == EventAction.fire)
                {
                    return true;
                }
                break;
            case WepeonType.Charge:
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
