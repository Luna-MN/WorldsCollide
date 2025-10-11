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
    
    public UiController UiController;
    public override void _Ready()
    {
        base._Ready();
        UiController = GetParent<UiController>();
        UiController.EquipmentSlots = this;
        var slots = GameManager.player.EquipmentSlots;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].EquippedEquipment != null)
            {
                var equipment = EquipmentUI.Instantiate<EquipmentUI>();
                equipment.AssignedEquipment = slots[i].EquippedEquipment;
                equipment.GlobalPosition = UISlots[i].GlobalPosition;
                equipment.UiController = GetParent<UiController>();
                equipment.selectedSlot = UISlots[i];
                equipment.StartingPos = equipment.selectedSlot;
                equipment.Icon.Texture = equipment.AssignedEquipment.Icon;
                UiController.AddChild(equipment);
            }
        }
    }

    public override void _ExitTree()
    {
        UiController.EquipmentSlots = null;
    }
}
