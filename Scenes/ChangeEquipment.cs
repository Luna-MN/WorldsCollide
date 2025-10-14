using Godot;
using System;

public partial class ChangeEquipment : Button
{
    [Export]
    public PackedScene EquipmentSlotsUI;
    [Export]
    public UiController UiController;
    public override void _Ready()
    {
        ButtonDown += onButtonPressed;
    }

    private void onButtonPressed()
    {
        if (UiController.EquipmentSlots != null) return;
        GameManager.UIOpen = true;
        UiController.EquipmentSlots = EquipmentSlotsUI.Instantiate<EquipmentSlots>();
        UiController.AddChild(UiController.EquipmentSlots);
        UiController.MoveChild(UiController.EquipmentSlots, 0);
    }
}
