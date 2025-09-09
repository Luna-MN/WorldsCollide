using Godot;
using System;


[GlobalClass]
public partial class BaseEquipment : Resource
{
    [ExportGroup("Flags")]
    [Export]
    public Flags.AbilityFlags equipmentFlags;
    
    [ExportGroup("Enhancements")]
    [Export(PropertyHint.ResourceType)]
    public BaseEnhancement[] enhancement;

    public void OnEquip(Character character)
    {
        foreach (var enhance in enhancement)
        {
            enhance.Enhance(character);
        }
    }
}