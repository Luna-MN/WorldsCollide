using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EquipmentUI : Panel
{
    [Export] public Area2D area;
    [Export] public Sprite2D Icon;
    public bool mouseIn;
    public bool mouseClick;
    public EquipmentSlotUI selectedSlot;
    public GridContainer grid;
    public EquipmentSelection TopUI;
    public BaseEquipment AssignedEquipment;
    public override void _Ready()
    {
        area.MouseEntered += () =>
        {
            mouseIn = true;
        };
        area.MouseExited += () =>
        {
            mouseIn = false;
        };
        area.AreaEntered += OnEquipSlotEnter;
        area.AreaExited += OnEquipSlotExit;

    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton MB)
        {
            if (!MB.Pressed)
            {
                mouseClick = false;
                if (selectedSlot != null)
                {
                    GlobalPosition = selectedSlot.GlobalPosition;
                    GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(selectedSlot)].EquippedEquipment = AssignedEquipment;
                    List<BaseEquipment> inv = GameManager.player.inventory.AllEquipment.ToList();
                    inv.Remove(AssignedEquipment);
                    GameManager.player.inventory.AllEquipment = inv.ToArray();
                }
                else
                {
                    List<BaseEquipment> inv = GameManager.player.inventory.AllEquipment.ToList();
                    inv.Add(AssignedEquipment);
                    GameManager.player.inventory.AllEquipment = inv.ToArray();
                    grid.CallDeferred("add_child", this);
                    GetParent().RemoveChild(this);
                }
            }
            if (MB.Pressed && mouseIn)
            {
                TopUI.CallDeferred("add_child", this);
                GetParent().RemoveChild(this);
                mouseClick = true;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (mouseClick)
        {
            GlobalPosition = GetGlobalMousePosition() + new Vector2(-45, -45);
        }
        

    }

    public void OnEquipSlotEnter(Node2D body)
    {
        if (body.GetParent() is EquipmentSlotUI)
        {
            var slot =  body.GetParent<EquipmentSlotUI>();
            if((GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(slot)].equipmentFlags & AssignedEquipment.equipmentFlags) != 0)
            {
                selectedSlot = body.GetParent<EquipmentSlotUI>();
            }
        }
    }

    public void OnEquipSlotExit(Node2D body)
    {
        if (selectedSlot == body.GetParent<EquipmentSlotUI>())
        {
            if (GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(selectedSlot)].EquippedEquipment == AssignedEquipment)
            {
                GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(selectedSlot)].EquippedEquipment = null;
            }
            selectedSlot = null;
        }
    }
}
