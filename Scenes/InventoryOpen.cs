using Godot;
using System;

public partial class InventoryOpen : Button
{
    [Export]
    public PackedScene InventoryUI;
    [Export]
    public UiController UiController;
    public override void _Ready()
    {
        ButtonDown += () =>
        {
            if (UiController.InventoryGrid == null)
            {
                UiController.InventoryGrid = InventoryUI.Instantiate<InventoryGrid>();
                UiController.AddChild(UiController.InventoryGrid);
                UiController.MoveChild(UiController.InventoryGrid, 0);
            }
        };
    }
}
