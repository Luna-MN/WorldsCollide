using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class PrimaryWeapon : BaseEquipment
{
    [Export] public string WeaponName;
    [Export] public string Description;
    [Export] public bool HasRightClick;
    [Export] public int Damage;
    [Export] public int Range;
    [Export] public float AttackSpeed;
    [Export] public Vector2 ShootPos;
    [Export] public Vector2 FlippedPos;
    [Export] public SpriteFrames SpriteFrames;
    public bool TwoHandedMode;
    public override void OnEquip(Character character)
    {
        if (equipmentFlags.HasFlag(Flags.AbilityFlags.TwoHanded))
        {
            GD.Print("Is Two Handed");
            character.PrimaryEquipment.Add(this);
            character.SecondaryEquipment.Add(this);
        }
        else if (equipmentFlags.HasFlag(Flags.AbilityFlags.MainHand))
        {
            character.PrimaryEquipment.Add(this);
        }
        else if(equipmentFlags.HasFlag(Flags.AbilityFlags.OffHand))
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
    
}
