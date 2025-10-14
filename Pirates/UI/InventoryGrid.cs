using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryGrid : MovableObject
{
    [Export]
    public PackedScene Item;
    [Export]
    public GridContainer Grid;
    public UiController UiController;
    public List<EquipmentUI> equipments = new();
    public override void _Ready()
    {
        base._Ready();
        UiController = GetParent<UiController>();
        UiController.InventoryGrid = this;
        foreach (var item in GameManager.player.inventory.AllEquipment)
        {
            var obj = Item.Instantiate<EquipmentUI>();
            obj.AssignedEquipment = item;
            obj.UiController = GetParent<UiController>();
            if (item.Icon != null)
            {
                obj.Icon.Texture = item.Icon;
            }
            equipments.Add(obj);
            Grid.AddChild(obj);
            
        }
    }

    public override void _ExitTree()
    {
        UiController.InventoryGrid = null;
        base._ExitTree();
    }

    public override void Close()
    {
        foreach (var equip in equipments)
        {
            equip.QueueFree();
        }
        base.Close();
    }
}
