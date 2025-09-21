using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EquipmentSelection : Panel
{
    [Export] public PackedScene EquipmentScene;
    [Export] public GridContainer EquipmentGrid;
    [Export] public EquipmentSlotUI[] EquipmentSlots;
    [Export] public Button CloseButton;
    public ChangeEquipment openButton;
    
    public override void _Ready()
    {
        foreach (var equipment in GameManager.player.inventory.AllEquipment)
        {
            var obj = EquipmentScene.Instantiate<EquipmentUI>();
            obj.AssignedEquipment = equipment;
            obj.grid = EquipmentGrid;
            obj.TopUI = this;
            if (equipment.Icon != null)
            {
                obj.Icon.Texture = equipment.Icon;
            }
            EquipmentGrid.AddChild(obj);
        }

        foreach (var slot in GameManager.player.EquipmentSlots)
        {
            if (slot.EquippedEquipment != null)
            {
                var obj = EquipmentScene.Instantiate<EquipmentUI>();
                obj.AssignedEquipment = slot.EquippedEquipment;
                obj.grid = EquipmentGrid;
                obj.TopUI = this;
                if (slot.EquippedEquipment.Icon != null)
                {
                    obj.Icon.Texture = slot.EquippedEquipment.Icon;
                }
                obj.selectedSlot = EquipmentSlots[GameManager.player.EquipmentSlots.ToList().IndexOf(slot)];
                AddChild(obj);
                obj.GlobalPosition = obj.selectedSlot.GlobalPosition;
            }
        }
        CloseButton.ButtonDown += () =>
        {
            openButton.EquipmentPanel = null; 
            GameManager.player.equipAll();
            
            var invIds = new List<int>();
            foreach (var equipment in GameManager.player.inventory.AllEquipment)
            {
                var item = GameManager.EquipmentRpcs.equipment.ToList().First(x => x.ResourceName.Contains(equipment.ResourceName));
                invIds.Add(GameManager.EquipmentRpcs.equipment.ToList().IndexOf(item));
            }
            var equippedIds = new List<int>();
            foreach (var slot in GameManager.player.EquipmentSlots)
            {
                if (slot.EquippedEquipment != null)
                {
                    var item = GameManager.EquipmentRpcs.equipment.ToList().First(x => x.ResourceName.Contains(slot.EquippedEquipment.ResourceName));
                    equippedIds.Add(GameManager.EquipmentRpcs.equipment.ToList().IndexOf(item));
                }
                else
                {
                    equippedIds.Add(-1);
                }
            }
            GameManager.ServerRpcs.RpcId(1, "UpdatePlayerEquipment", GameManager.LocalID, equippedIds.ToArray(), invIds.ToArray());
            QueueFree();
        };
    }
}
