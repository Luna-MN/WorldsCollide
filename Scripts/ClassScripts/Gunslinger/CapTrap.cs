using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CapTrap : Minion
{
    public override void _Ready()
    {
        if (!Multiplayer.IsServer())
        {

            ID = Name.ToString();
            var split = ID.Split('_');
            GD.Print(split[0]);
            GD.Print(string.Join(",", GameManager.Players.Select(x => x.ID)));
            var player = GameManager.Players.First(x => x.ID == split[0]);
            EquipmentSlots[0].EquippedEquipment = player.EquipmentSlots[0].EquippedEquipment;
            equipAll();
        }
        base._Ready();
    }
}
