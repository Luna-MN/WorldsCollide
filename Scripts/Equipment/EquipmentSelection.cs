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
    public bool pickedUp = false;
    
    public override void _Ready()
    {
        foreach (var equipment in GameManager.player.inventory.AllEquipment)
        {
            var obj = EquipmentScene.Instantiate<EquipmentUI>();
            obj.AssignedEquipment = equipment;
            obj.grid = EquipmentGrid;
            // obj.UiController = this;
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
                // obj.UiController = this;
                if (slot.EquippedEquipment.Icon != null)
                {
                    obj.Icon.Texture = slot.EquippedEquipment.Icon;
                }
                obj.selectedSlot = EquipmentSlots[GameManager.player.EquipmentSlots.ToList().IndexOf(slot)];
                obj.selectedSlot.equip = obj;
                if (slot.EquippedEquipment is PrimaryWeapon { TwoHandedMode: true })
                {
                    var otherIndex = GameManager.player.EquipmentSlots.ToList().IndexOf(slot) == 0 ? 1 : 0;
                    EquipmentSlots[otherIndex].Blocked = true;
                    obj.dummy = obj.DummyScene.Instantiate<DummySlot>();
                    obj.dummy.Icon.Texture = obj.Icon.Texture;
                    AddChild(obj.dummy);
                    obj.dummy.GlobalPosition = EquipmentSlots[otherIndex].GlobalPosition;
                }
                AddChild(obj);
                obj.GlobalPosition = obj.selectedSlot.GlobalPosition;
            }
        }
        CloseButton.ButtonDown += () =>
        {
            openButton.EquipmentPanel = null; 
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
            QueueFree();
        };
    }
}
