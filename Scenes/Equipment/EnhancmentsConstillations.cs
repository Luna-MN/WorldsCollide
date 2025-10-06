using Godot;
using System;
using System.Collections.Generic;

public partial class EnhancmentsConstillations : Control
{
    [Export] public PackedScene star;
    [Export] public ConstillationData constellation;
    public List<EnhancmentStar> stars = new();
    
    private ConstellationLines lineDrawer;
    private Random random = new Random();

    public override void _Ready()
    {
        // Create the line drawer first
        lineDrawer = new ConstellationLines();
        AddChild(lineDrawer);
        lineDrawer.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        foreach (var slot in constellation.ConstillationSlots)
        {
            var createdSlot = star.Instantiate<EnhancmentStar>();
            createdSlot.data = slot;
            
            createdSlot.Position = new Vector2(
                random.Next(0, (int)Size.X),
                random.Next(0, (int)Size.Y)
            );

            stars.Add(createdSlot);
            AddChild(createdSlot);
        }

        // Set up connections
        for (int i = 0; i < stars.Count; i++)
        {
            var star = stars[i];
            if(star.data.connectionIndexes == null || star.data.connectionIndexes.Length == 0) continue;
            
            foreach (var connection in star.data.connectionIndexes)
            {
                lineDrawer.connections.Add((i, connection));
            }
        }

        lineDrawer.stars = stars;
    }

    public override void _Process(double delta)
    {
        // Redraw lines when stars move
        if (lineDrawer != null)
        {
            lineDrawer.QueueRedraw();
        }
    }
}