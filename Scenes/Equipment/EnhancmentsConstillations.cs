using Godot;
using System;

public partial class EnhancmentsConstillations : Control
{
    [Export] public PackedScene star;
    [Export] public PackedScene constellationLine;
    [Export] public ConstillationData constellation;
    public override void _Ready()
    {
        foreach (var slot in constellation.ConstillationSlots)
        {
            var createdSlot = star.Instantiate();
            AddChild(createdSlot);
        }
    }
}
