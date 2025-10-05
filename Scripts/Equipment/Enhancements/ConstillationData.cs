using Godot;
using System;

public partial class ConstillationData : Resource
{
    [Export(PropertyHint.ResourceType)] public ConstillationSlotData[] ConstillationSlots;
}
