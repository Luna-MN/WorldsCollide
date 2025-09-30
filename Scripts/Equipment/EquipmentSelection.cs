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
            
            var invIds = new List<int>();
            foreach (var equipment in GameManager.player.inventory.AllEquipment)
            {
                var item = GameManager.EquipmentRpcs.equipment.ToList().First(x => x.ResourceName.Contains(equipment.ResourceName));
                invIds.Add(GameManager.EquipmentRpcs.equipment.ToList().IndexOf(item));
            }
            var equippedIds = new List<int>();
            var enhancements = new List<List<int>>();
            foreach (var slot in GameManager.player.EquipmentSlots)
            {
                var slotIndex = GameManager.player.EquipmentSlots.ToList().IndexOf(slot);
                if (slot.EquippedEquipment != null)
                {
                    var item = GameManager.EquipmentRpcs.equipment.ToList().First(x => x.ResourceName.Contains(slot.EquippedEquipment.ResourceName));
                    var enhancmentsTemp = new List<int>();
                    item.enhancements.ToList().ForEach(x => enhancmentsTemp.Add(GameManager.EquipmentRpcs.Enhancments.ToList().FindIndex(y => y.ResourceName == x.ResourceName)));
                    enhancements.Add(enhancmentsTemp);
                    equippedIds.Add(GameManager.EquipmentRpcs.equipment.ToList().IndexOf(item));
                }
                else
                {
                    equippedIds.Add(-1);
                    enhancements.Add(new List<int>());
                }
            }
            Godot.Collections.Dictionary<int, int[]> EqippedIds = new Godot.Collections.Dictionary<int, int[]>();
            for (var i = 0; i < equippedIds.Count; i++)
            {
                EqippedIds.Add(i, enhancements[i].ToArray());
            }

            GameManager.ServerRpcs.RpcId(1, "UpdatePlayerEquipment", GameManager.LocalID,equippedIds.ToArray() ,EqippedIds, invIds.ToArray());
            GameManager.UIOpen = false;
            QueueFree();
        };
    }
}
