using Godot;
using System;

public partial class EquipmentSelection : Panel
{
    [Export] public PackedScene EquipmentScene;
    [Export] public GridContainer EquipmentGrid;
    [Export] public EquipmentSlotUI[] EquipmentSlots;
    public ChangeEquipment openButton;
    
    public override void _Ready()
    {
        foreach (var equipment in GameManager.player.inventory.AllEquipment)
        {
            var obj = EquipmentScene.Instantiate<EquipmentUI>();
            obj.AssignedEquipment = equipment;
            EquipmentGrid.AddChild(obj);
        }
    }
}
