using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EquipmentGenerator : Node2D
{
    [Export]
    public PackedScene FloorItem;
    [Export]
    public Godot.Collections.Dictionary<Rarity, Color[]> Colors;
    public int Level;
    public float Prestige;
    public List<int> CharacterIds;
    public List<BaseEquipment> GenerationEquipment;
    public float GenerateStep = 0.85f;
    public float GeneratePercentage = 1f;
    
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
        AddToGroup("EquipmentGenerators");
    }

    public override void _Process(double delta)
    {
        rng.Randomize();
        foreach (var Id in CharacterIds.ToList())
        {
            var RandPercentage = (float)Mathf.Snapped(rng.RandfRange(0, 1), 0.0001);
            if(RandPercentage <= GeneratePercentage)
            {
                GD.Print("Generating " + GeneratePercentage);
                RandPercentage = rng.RandfRange(0, 1);
                var ItemRarity = RaritySwitch(RandPercentage);
                var colors = Colors[ItemRarity];
                var quality = (Level * 50) + (Prestige) + ((float)Math.Pow(1.8, (int)ItemRarity) * 25);
                var actualQuality = rng.RandiRange((int)Mathf.Min(0, quality-100), (int)quality+100);
                
                var floorItem = FloorItem.Instantiate<FloorEquipmentDefault>();
                floorItem.colors = colors;
                GD.Print(ItemRarity);
                if (GenerationEquipment.Count > 0)
                {
                    var equipment = GenerationEquipment[rng.RandiRange(0, GenerationEquipment.Count)];
                    var enhancments = (actualQuality / 1000) + 1;
                    for (int i = 0; i < enhancments; i++)
                    {
                        // generate enhancments
                    }
                    floorItem.equipment = equipment;
                }
                floorItem.GlobalPosition = GlobalPosition;
                
                // Generate random angle in radians
                float randomAngle = (float)(rng.RandfRange(0, Mathf.Tau)); // Tau is 2*PI
                float radius = 100f; // Adjust this radius as needed

                // Set position using sine and cosine to create circular pattern
                floorItem.FinalPosition = GlobalPosition + new Vector2(
                    Mathf.Cos(randomAngle) * radius,
                    Mathf.Sin(randomAngle) * radius
                );
                GetParent().AddChild(floorItem, true);
            }
            else
            {
                CharacterIds.Remove(Id);
            }
            GeneratePercentage *= GenerateStep;
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
