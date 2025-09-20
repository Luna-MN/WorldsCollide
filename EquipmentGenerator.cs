using Godot;
using System;
using System.Collections.Generic;

public partial class EquipmentGenerator : Node2D
{
    [Export]
    public PackedScene FloorItem;
    public int Level;
    public float Prestige;
    public List<int> CharacterIds;
    
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Pristine,
        Legendary,
        Godly
    }
    // I need a list of Ids to generate items for
    public override void _Process(double delta)
    {
        
    }
}
