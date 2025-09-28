using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EquipmentUI : Panel
{
    [Export] public Area2D area;
    [Export] public Sprite2D Icon;
    [Export] public PackedScene HoverScene;
    [Export] public PackedScene DummyScene;
    public DummySlot dummy;
    public EquipmentHoverScene scene;
    public bool mouseIn;
    public bool mouseClick;
    public bool JustCreated = true;
    public Timer timer;
    public EquipmentSlotUI selectedSlot;
    public GridContainer grid;
    public EquipmentSelection TopUI;
    public BaseEquipment AssignedEquipment;
    public override void _Ready()
    {
        area.MouseEntered += () =>
        {
            if (!mouseIn && scene == null)
            {
                scene = HoverScene.Instantiate<EquipmentHoverScene>();
                scene.GlobalPosition = GetGlobalMousePosition();
                scene.ItemIcon.Texture = Icon.Texture;
                scene.ItemName.Text = AssignedEquipment.ResourceName;
                scene.ItemName.Modulate = scene.TextColors[(int)AssignedEquipment.Rarity];
                scene.ItemDescription.Text =
                    string.Join('\n', AssignedEquipment.enhancements.Select(x => x.EnhancmentText));
                scene.Quality.Text = AssignedEquipment.Quality.ToString();
                if (AssignedEquipment is PrimaryWeapon p)
                {
                    scene.wepStats.Visible = true;
                    scene.Attack.Text = scene.Attack.Text.Replace("xx", p.Damage.ToString());
                    scene.Speed.Text = scene.Speed.Text.Replace("xx", p.AttackSpeed.ToString());
                    scene.Range.Text = scene.Range.Text.Replace("xx", p.Range.ToString());
                }
                
                TopUI.AddChild(scene);
            }
            mouseIn = true;
        };
        area.MouseExited += () =>
        {
            
            mouseIn = false;
            if (scene != null)
            {
                scene.QueueFree();
                scene = null;
            }
        };
        area.AreaEntered += OnEquipSlotEnter;
        area.AreaExited += OnEquipSlotExit;
        timer = new Timer()
        {
            Autostart = true,
            OneShot = true,
            WaitTime = 0.5f
        };
        timer.Timeout += () =>
        {
            JustCreated = false;
        };
        AddChild(timer);
    }

    public override void _Input(InputEvent @event)
    {
        if (JustCreated)
        {
            return; 
        }
        if (@event is InputEventMouseButton MB)
        {
            if (!MB.Pressed && mouseClick)
            {
                mouseClick = false;
                TopUI.EquipmentSlots.Where(x => (GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(x)].equipmentFlags & AssignedEquipment.equipmentFlags) != 0).ToList().ForEach(x => x.Modulate = new Color(1, 1, 1));
                // Add equipment to slot
                if (selectedSlot != null)
                {
                    if (selectedSlot.equip != null && selectedSlot.equip != this)
                    {
                        var equipThere = selectedSlot.equip;
                        equipThere.JustCreated = true;
                        equipThere.timer.Start();
                        if (equipThere.dummy != null)
                        {
                            equipThere.dummy.QueueFree();
                        }
                        var invent = GameManager.player.inventory.AllEquipment.ToList();
                        invent.Add(AssignedEquipment);
                        GameManager.player.inventory.AllEquipment = invent.ToArray();
                        grid.CallDeferred("add_child", equipThere);
                        GetParent().RemoveChild(equipThere);
                    }
                    //one handed wep
                    selectedSlot.equip = this;
                    GlobalPosition = selectedSlot.GlobalPosition;
                    GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(selectedSlot)].EquippedEquipment = AssignedEquipment;
                    List<BaseEquipment> inv = GameManager.player.inventory.AllEquipment.ToList();
                    inv.Remove(AssignedEquipment);
                    GameManager.player.inventory.AllEquipment = inv.ToArray();
                    // If assigning to main hand then we check if the wep is 2 handed, if so then we add a dummy panel to the offhand slot and block it
                    if (TopUI.EquipmentSlots.ToList().FindIndex(x => x == selectedSlot) == 0 && (AssignedEquipment.equipmentFlags & Flags.AbilityFlags.TwoHanded) != 0)
                    {
                        TopUI.EquipmentSlots[1].Blocked = true;
                        dummy = DummyScene.Instantiate<DummySlot>();
                        dummy.Icon.Texture = Icon.Texture;
                        TopUI.AddChild(dummy);
                        dummy.GlobalPosition = TopUI.EquipmentSlots[1].GlobalPosition;
                    }
                }
                // Remove from equipment slot
                else
                {
                    List<BaseEquipment> inv = GameManager.player.inventory.AllEquipment.ToList();
                    List<int> equipIds = inv.Select(x => x.ItemId).ToList();
                    
                    // it is finding other peices of equipment with the same base resource, might need to add an ID system
                    if (!equipIds.Contains(AssignedEquipment.ItemId))
                    {
                        if (GameManager.player.EquipmentSlots.Select(x => x.EquippedEquipment).Any(x => x == AssignedEquipment))
                        {
                            GameManager.player.EquipmentSlots.First(x => x.EquippedEquipment == AssignedEquipment).EquippedEquipment = null;
                        }

                        inv.Add(AssignedEquipment);
                        GameManager.player.inventory.AllEquipment = inv.ToArray();
                    }
                    grid.CallDeferred("add_child", this);
                    GetParent().RemoveChild(this);
                }
            }
            if (MB.Pressed && mouseIn)
            {
                TopUI.CallDeferred("add_child", this);
                if (scene != null)
                {
                    scene.QueueFree();
                    scene = null;
                }
                GetParent().RemoveChild(this);
                TopUI.EquipmentSlots.Where(x => (GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(x)].equipmentFlags & AssignedEquipment.equipmentFlags) != 0).ToList().ForEach(x => x.Modulate = new Color(0, 0.5f, 1));
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
        if (body.GetParent() is EquipmentSlotUI slot && !JustCreated)
        {
            if ((GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(slot)].equipmentFlags & AssignedEquipment.equipmentFlags) != 0 && !slot.Blocked)
            {
                if (selectedSlot != null && selectedSlot != slot)
                {
                    GameManager.player.EquipmentSlots[TopUI.EquipmentSlots.ToList().IndexOf(selectedSlot)].EquippedEquipment = null;
                    if (selectedSlot.equip == this)
                    {
                        selectedSlot.equip = null;
                    }
                    selectedSlot = null;
                }
                selectedSlot = body.GetParent<EquipmentSlotUI>();
                GD.Print("Selected Slot = " + TopUI.EquipmentSlots.ToList().IndexOf(selectedSlot));
            }
        }
    }

    public void OnEquipSlotExit(Node2D body)
    {
        if (body.GetParent() is not EquipmentSlotUI)
        {
            return;
        }
        if (selectedSlot == body.GetParent<EquipmentSlotUI>())
        {
            if (selectedSlot.equip == this)
            {
                //delete dummy if it exists
                if (dummy != null)
                {
                    dummy.QueueFree();
                    dummy = null;
                    TopUI.EquipmentSlots[1].Blocked = false;
                }
                selectedSlot.equip = null;
            }
            selectedSlot = null;
        }
    }
}
