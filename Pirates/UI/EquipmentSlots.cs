using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class EquipmentSlots : MovableObject
{
    [Export]
    public EquipmentSlotUI[] UISlots;
    [Export] 
    public PackedScene EquipmentUI;
    public override void _Ready()
    {
        var slots = GameManager.player.EquipmentSlots;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].EquippedEquipment != null)
            {
                var equipment = EquipmentUI.Instantiate<EquipmentUI>();
                equipment.AssignedEquipment = slots[i].EquippedEquipment;
                equipment.GlobalPosition = UISlots[i].GlobalPosition;
                
            }
        }
    }
}
