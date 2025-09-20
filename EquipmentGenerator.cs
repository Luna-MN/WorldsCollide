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
    public float GeneratePercentage = 0.85f;
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
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    public override void _Ready()
    {
        rng.Randomize();
    }

    public override void _Process(double delta)
    {
        foreach (var Id in CharacterIds)
        {
            var RandPercentage = (float)Mathf.Snapped(rng.RandfRange(0, 1), 0.0001);
            
            if(RandPercentage >= GeneratePercentage)
            {
                RandPercentage = rng.RandfRange(0, 1);
                var ItemRarity = RaritySwitch(RandPercentage);
                
            }
            else
            {
                QueueFree();
            }
        }
    }

    public Rarity RaritySwitch(float RarityPercentage)
    {
        switch (RarityPercentage)
        {
            case < 0.45f:
                return Rarity.Common;
            case < 0.75f:
                return Rarity.Uncommon;
            case < 0.9f:
                return Rarity.Rare;
            case < 0.965f:
                return Rarity.Epic;
            case < 0.994f:
                return Rarity.Pristine;
            case < 0.999f:
                return Rarity.Legendary;
            case < 1f:
                return Rarity.Godly;
            default:
                return Rarity.Common;
        }
    }
}
