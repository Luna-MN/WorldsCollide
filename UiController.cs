using Godot;
using System;
using System.Collections.Generic;

public partial class UiController : Control
{
    public bool pickedUp;
    public EquipmentSlots EquipmentSlots;
    public InventoryGrid InventoryGrid;
    [Export]
    public Control DragLayer;
    public List<MovableObject> Objects = new();
    public MovableObject SelectedObject;
}
