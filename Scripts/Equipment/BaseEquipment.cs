using Godot;
using System;
using System.Linq;


[GlobalClass]
public partial class BaseEquipment : Resource
{
    [ExportGroup("Flags")]
    [Export]
    public Flags.AbilityFlags equipmentFlags;
    [Export] public Stats stats;
    [ExportGroup("Enhancements")]
    [Export(PropertyHint.ResourceType)]
    public BaseEnhancement[] enhancements;
    [Export] public Vector2 Scale = new Vector2(0.875f, 0.875f);
    [Export]
    public Texture2D Icon;
    public float Quality;
    public EquipmentGenerator.Rarity Rarity;
    [Export]
    public int ItemId;
    public virtual void OnEquip(Character character)
    {
        foreach (var enhance in enhancements)
        {
            enhance.Enhance(character);
        }
    }
}