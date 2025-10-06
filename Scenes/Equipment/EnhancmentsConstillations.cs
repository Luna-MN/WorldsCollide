using Godot;
using System;

public partial class EnhancmentsConstillations : Control
{
    [Export] public PackedScene star;
    [Export] public PackedScene constellationLine;
    [Export] public ConstillationData constellation;
    public EnhancmentStar[] stars;
    public override void _Ready()
    {
        foreach (var slot in constellation.ConstillationSlots)
        {
            var createdSlot = star.Instantiate<EnhancmentStar>();
            createdSlot.data = slot;
            AddChild(createdSlot);
        }

        foreach (var star in stars)
        {
            foreach (var connection in star.data.connectionIndexes)
            {
                var line = constellationLine.Instantiate<EnhancmentStarLine>();
                line.startStar = star;
                line.endStar = stars[connection];
                AddChild(line);
            }
        }
    }
}
