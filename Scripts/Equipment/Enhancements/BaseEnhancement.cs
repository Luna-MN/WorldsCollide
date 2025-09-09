using Godot;
using System;

[GlobalClass]
public partial class BaseEnhancement : Resource
{
    public string EnhancmentText;


    public virtual void Enhance(Character character)
    {
        //add what you want it to do here
        // so like character.OnHit += something
    }
}
