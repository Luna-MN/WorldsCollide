using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    private Timer timer;
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
        timer = new Timer()
        {
            WaitTime = 0.3f,
            OneShot = false,
            Autostart = true
        };
        timer.Timeout += () =>
        {
            // add some delay to add some spice
            GenerateEquipment();
        };
        AddChild(timer);
        }

    public override void _Process(double delta)
    {
        if (CharacterIds.Count == 0)
        {
            // delete when no longer spawning
            QueueFree();
        }
    }

    public void GenerateEquipment()
    {
        // randomize the rng
        rng.Randomize();
        // try generate a item for each character
        foreach (var Id in CharacterIds.ToList())
        {
            // snapped rounds it to that many DP ending in a number 0.xxxx
            var RandPercentage = (float)Mathf.Snapped(rng.RandfRange(0, 1), 0.0001);
            if(RandPercentage <= GeneratePercentage)
            {
                GD.Print("Generating " + GeneratePercentage);
                RandPercentage = rng.RandfRange(0, 1);
                // get rarity
                var ItemRarity = RaritySwitch(RandPercentage);
                // convert rarity into the color
                var colors = Colors[ItemRarity];
                // get quality formula on notion if its annoying to read here
                var quality = (Level * 50) + (Prestige) + ((float)Math.Pow(1.8, (int)ItemRarity) * 25);
                // adjust quality to be between -100 and 100
                var actualQuality = rng.RandiRange((int)Mathf.Max(0, quality-100), (int)quality+100);
                
                // generate floor item
                var floorItem = FloorItem.Instantiate<FloorEquipmentDefault>();
                floorItem.colors = colors;
                if (GenerationEquipment.Count > 0)
                {
                    // Generate a random equipment
                    var RandomEquipment = rng.RandiRange(0, GenerationEquipment.Count - 1);
                    GD.Print(RandomEquipment);
                    var equipment = GenerationEquipment[RandomEquipment];
                    equipment.Rarity = ItemRarity;
                    equipment.Quality = actualQuality;
                    // the number of enhancments is equal to the first number for the quality
                    var enhancmentsNum = (actualQuality / 1000) + 1;
                    // get all creatable enhancments based on flags and rarity level
                    var creatableEnhancments = ServerManager.EquipmentRpcs.Enhancments.Where(x => (x.MinEnhancmentLevel >= (int)ItemRarity) && (x.EnhancmentFlags & equipment.equipmentFlags) != 0).ToList();
                    var enhancments = new List<BaseEnhancement>();
                    // convert the quality into a percentage
                    var qualPerc = actualQuality / 6630;
                    if (creatableEnhancments.Count > 0)
                    {
                        for (int i = 0; i < enhancmentsNum; i++)
                        {
                            var en = creatableEnhancments[rng.RandiRange(0, creatableEnhancments.Count - 1)];
                            if (en.ValueBased)
                            {
                                // of the enhancment is value based then get the percentage of max and then convert that into the value 
                                en.Value = en.MinValue + (en.MaxValue - en.MinValue) * qualPerc;
                            }

                            enhancments.Add(en);
                        }
                    }
                    foreach (var stat in equipment.stats.stats)
                    {
                        if (stat.Value.ClampValue)
                        {
                            // clamp the value between the min and max
                            stat.Value.CalcValue = stat.Value.MinValue + (stat.Value.MaxValue - stat.Value.MinValue) * qualPerc;
                        }
                    }
                    equipment.enhancements = enhancments.ToArray();
                    floorItem.equipment = equipment;
                }

                floorItem.Id = Id;
                floorItem.GlobalPosition = GlobalPosition;
                
                // Generate random angle in radians
                float randomAngle = (float)(rng.RandfRange(0, Mathf.Tau)); // Tau is 2*PI
                float radius = rng.RandiRange(70, 150); // Adjust this radius as needed

                // Set position using sine and cosine to create circular pattern
                floorItem.FinalPosition = GlobalPosition + new Vector2(
                    Mathf.Cos(randomAngle) * radius,
                    Mathf.Sin(randomAngle) * radius
                );
                floorItem.equipment.ItemId = (int)(new RandomNumberGenerator()).Randi();
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
