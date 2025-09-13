using Godot;
using System;

public partial class ChangeEquipment : Button
{
    [Export]
    public PackedScene EquipmentUI;
    public EquipmentSelection EquipmentPanel;
    public override void _Ready()
    {
        ButtonDown += onButtonPressed;
    }

    private void onButtonPressed()
    {
        if (EquipmentPanel != null) return;
        EquipmentPanel = EquipmentUI.Instantiate<EquipmentSelection>();
        EquipmentPanel.openButton = this;
        GetParent().AddChild(EquipmentPanel);
    }
}
