using Godot;
using System;

[GlobalClass]
public partial class BaseEnhancement : Resource
{
    [Export]
    public string EnhancmentText;
    [Export]
    public Flags.AbilityFlags EnhancmentFlags;
    [Export(PropertyHint.Range, "1, 7, 1")]
    public int MinEnhancmentLevel;
    
    public virtual void Enhance(Character character)
    {
        //add what you want it to do here
        // so like character.OnHit += something
    }
}
