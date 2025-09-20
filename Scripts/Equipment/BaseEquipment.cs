using Godot;
using System;
using System.Linq;


[GlobalClass]
public partial class BaseEquipment : Resource
{
    [ExportGroup("Flags")]
    [Export]
    public Flags.AbilityFlags equipmentFlags;
    
    [ExportGroup("Enhancements")]
    [Export(PropertyHint.ResourceType)]
    public BaseEnhancement[] enhancements;
    
    [Export]
    public Texture2D Icon;
    public float Quality;
    
    public virtual void OnEquip(Character character)
    {
        foreach (var enhance in enhancements)
        {
            enhance.Enhance(character);
        }
    }
}