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
            var inv = new List<BaseEquipment>();
            GameManager.player.inventory.AllEquipment.ToList().ForEach(x => inv.Add(GameManager.EquipmentRpcs.equipment.ToList().FirstOrDefault(y => x.ResourcePath.Contains(y.ResourcePath))));
            var InvIds = inv.Select(x => GameManager.EquipmentRpcs.equipment.ToList().IndexOf(x)).ToArray();
            
            var equipped = GameManager.player.EquipmentSlots.Select(x => x.EquippedEquipment).ToList();
            var equippedIds = equipped.Select(x => GameManager.EquipmentRpcs.equipment.ToList().IndexOf(x)).ToList();
            GameManager.ServerRpcs.RpcId(1, "UpdatePlayerEquipment", GameManager.LocalID, equippedIds.ToArray(), InvIds.ToArray());
            QueueFree();
        };
    }
}
