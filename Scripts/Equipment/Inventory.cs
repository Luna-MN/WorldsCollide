using Godot;
using System;
[GlobalClass]
public partial class Inventory : Resource
{
    [Export(PropertyHint.ResourceType)]
    public BaseEquipment[] AllEquipment;
}
