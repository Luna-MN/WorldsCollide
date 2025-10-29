using Godot;
using System;

public partial class EnhancementSelection : MovableObject
{
    [Export]
    public EquipmentSlotUI UISlot;
    [Export]
    public EnhancmentsConstillations constellation;
    public bool reloading;
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (UISlot.equip != null && reloading == false)
        {
            reloading = true;
            constellation.constellation = UISlot.equip.AssignedEquipment.EnhancementData;
            constellation.DrawStars();
        }
        else if (UISlot.equip == null)
        {
            reloading = false;
        }
    }
}
