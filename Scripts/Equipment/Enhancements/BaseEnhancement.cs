using Godot;
using System;
[Tool]
[GlobalClass]
public partial class BaseEnhancement : Resource
{
    [Export(PropertyHint.MultilineText, "if you put {Value} it will replace with the value")]
    public string EnhancmentText;
    [Export]
    public Flags.AbilityFlags EnhancmentFlags;
    [Export(PropertyHint.Range, "1, 7, 1")]
    public int MinEnhancmentLevel;
    [ExportGroup("Value Based")]
    [Export(PropertyHint.GroupEnable)]
    public bool ValueBased;
    public float Value = 10;
    [Export] public float MinValue;
    [Export] public float MaxValue;
    public virtual void Enhance(Character character)
    {
        //add what you want it to do here
        // so like character.OnHit += something
    }

    public string GetDescription()
    {
        return EnhancmentText.Replace("{Value}", Value.ToString());
    }
}
