using Godot;
using System;
[GlobalClass]
public partial class EquipmentSlot : Resource
{
    

    [Export]
    public BaseEquipment EquippedEquipment;
    [ExportGroup("Flags")]
    [Export]
    public Flags.AbilityFlags equipmentFlags;
    
}
