using Godot;
using System;
[GlobalClass]
public partial class PrimaryWeapon : BaseEquipment
{
    [Export] public string WeaponName;
    [Export] public string Description;
    [Export] public bool HasRightClick;
    [Export] public Texture2D Icon;
    [Export] public int Damage;
    [Export] public int Range;
    [Export] public int AttackSpeed;
    
    public void Left_Click()
    {
        GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_LeftClick", (int)GameManager.LocalID);
    }

    public void Right_Click()
    {
        GameManager.EquipmentRpcs.RpcId(1, WeaponName + "_RightClick", (int)GameManager.LocalID);
    }
    
}
