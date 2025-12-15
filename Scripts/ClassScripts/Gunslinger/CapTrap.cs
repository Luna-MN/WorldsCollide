using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CapTrap : Minion
{
    public override void _Ready()
    {
        var player = null as Character;
        if (!Multiplayer.IsServer())
        {

            ID = Name.ToString();
            var split = ID.Split('_');
            GD.Print(split[0]);
            GD.Print(string.Join(",", GameManager.Players.Select(x => x.ID)));
            player = GameManager.Players.First(x => x.ID == split[0]);
            equipAll();
        }
        else
        {
            player = ServerManager.NodeDictionary[ID.Split('_')[0]] as Character;
        }
        EquipmentSlots[0].EquippedEquipment = (PrimaryWeapon)player?.EquipmentSlots[0].EquippedEquipment.Duplicate(true);
        GD.Print(EquipmentSlots[0].EquippedEquipment);
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (TargetPosition != Vector2.Zero)
        {
            PrimaryEquipment[0].Attacks[0].Event = true;
        }
        else
        {
            PrimaryEquipment[0].Attacks[0].Event = false;
        }

        base._Process(delta);
        if (Multiplayer.IsServer())
        {
            return;
        }
        foreach (var wep in PrimaryEquipment)
        {
            if (wep.Attacks[0].Available && wep.Attacks[0].Event)
            {
                var attack = wep.Attack(this, true, wep.Attacks[0], PrimaryWeapon.EventAction.fire);
                if (attack)
                {
                    LeftClick(wep, TargetPosition);
                }
            }
        }
    }
}
