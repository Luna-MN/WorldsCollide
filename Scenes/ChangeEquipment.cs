using Godot;
using System;

public partial class ChangeEquipment : Button
{
    [Export]
    public PackedScene EquipmentSlotsUI;
    [Export]
    public PackedScene EquipmentGridUI;
    [Export]
    public UiController UiController;
    public EquipmentSlots EquipmentPanel;
    public InventoryGrid InventoryPanel;
    public override void _Ready()
    {
        ButtonDown += onButtonPressed;
    }

    private void onButtonPressed()
    {
        if (EquipmentPanel != null) return;
        GameManager.UIOpen = true;
        EquipmentPanel = EquipmentSlotsUI.Instantiate<EquipmentSlots>();
        UiController.EquipmentSlots = EquipmentPanel;
        UiController.AddChild(EquipmentPanel);
    }
}
