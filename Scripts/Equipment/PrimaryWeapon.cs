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
    [Export] public int AttackSpeed;
    [Export] public Vector2 GunPos;
    [Export] public Vector2 GunFlipped;
    [Export] public Vector2 ShootPos;
    [Export] public Vector2 FlippedPos;
    [Export] public SpriteFrames SpriteFrames;
    
    public override void OnEquip(Character character)
    {
        character.PrimaryEquipment.Add(this);
        base.OnEquip(character);
    }

    public void Left_Click()
    {
        GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_LeftClick", (int)GameManager.LocalID);
    }

    public void Right_Click()
    {
        GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_RightClick", (int)GameManager.LocalID);
    }
    
}
