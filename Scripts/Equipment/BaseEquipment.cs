using Godot;
using System;
using System.Linq;


[GlobalClass]
public partial class BaseEquipment : Resource
{
    [ExportGroup("Flags")]
    [Export]
    public Flags.AbilityFlags equipmentFlags;
    [ExportGroup("Stats")]
    [Export] public Stats stats;
    [ExportGroup("Enhancements")]
    [Export] public Vector2 Scale = new Vector2(0.875f, 0.875f);
    [Export]
    public Texture2D Icon;
    public float Quality;
    public EquipmentGenerator.Rarity Rarity;
    [Export]
    public int ItemId;
    [Export] public ConstillationData EnhancementData = new();
    
    public virtual void OnEquip(Character character)
    {
        if(EnhancementData.ConstillationSlots == null) return;
        foreach (var enhance in EnhancementData.ConstillationSlots.Select(x => x.Star).Where(x => x != null))
        {
            enhance.Enhance(character);
        }
    }
}