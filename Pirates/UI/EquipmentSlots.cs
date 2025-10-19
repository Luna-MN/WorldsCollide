using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class EquipmentSlots : MovableObject
{
    [Export]
    public EquipmentSlotUI[] UISlots;
    [Export] 
    public PackedScene EquipmentUI;
    
    public UiController UiController;
    
    public List<EquipmentUI> equipments = new();
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
                equipment.SlotAssigned = true;
                equipment.Icon.Texture = equipment.AssignedEquipment.Icon;
                equipment.Name = equipment.AssignedEquipment.ItemId.ToString();
                equipments.Add(equipment);
                UiController.AddChild(equipment);
            }
        }
    }

    public override void Close()
    {
        foreach (var equip in equipments)
        {
            equip.QueueFree();
        }
        GameManager.player.equipAll();
        var invIds = GameManager.player.inventory.AllEquipment.Select(x => x.ItemId).ToList();
        List<int> equippedIds = new();
        foreach (var slot in GameManager.player.EquipmentSlots)
        {
            if (slot.EquippedEquipment != null)
            {
                equippedIds.Add(slot.EquippedEquipment.ItemId);
            }
            else
            {
                equippedIds.Add(-1);
            }
        }
        GameManager.ServerRpcs.RpcId(1, "UpdatePlayerEquipment", Multiplayer.GetUniqueId(), equippedIds.ToArray() ,invIds.ToArray());
        GameManager.UIOpen = false;
        base.Close();
    }
    public override void _ExitTree()
    {
        base._ExitTree();
        UiController.EquipmentSlots = null;
    }
}
