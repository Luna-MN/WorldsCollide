using Godot;
using System;
[GlobalClass]
public partial class ConstillationData : Resource
{
    [Export(PropertyHint.ResourceType)] public ConstillationSlotData[] ConstillationSlots;
}
