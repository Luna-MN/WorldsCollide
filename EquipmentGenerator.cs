using Godot;
using System;

public partial class EquipmentGenerator : Node2D
{
    [Export]
    public PackedScene FloorItem;

    public int Level;
    
    public float Prestige;
    // I need a list of Ids to generate items for
    public override void _Process(double delta)
    {
        
    }
}
