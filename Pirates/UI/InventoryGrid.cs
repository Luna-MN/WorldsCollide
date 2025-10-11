using Godot;
using System;

public partial class InventoryGrid : MovableObject
{
    [Export]
    public PackedScene Item;
    [Export]
    public GridContainer Grid;
    public UiController UiController;
    public override void _Ready()
    {
        UiController = GetParent<UiController>();
        UiController.InventoryGrid = this;
        foreach (var item in GameManager.player.inventory.AllEquipment)
        {
            var obj = Item.Instantiate<EquipmentUI>();
            obj.AssignedEquipment = item;
            Grid.AddChild(obj);
        }
    }
}
